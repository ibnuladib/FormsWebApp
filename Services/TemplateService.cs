using System.Drawing.Text;
using FormsWebApplication.Data;
using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using FormsWebApplication.Models.FormsWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FormsWebApplication.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly FormsWebAppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LuceneSearchService _luceneSearchService;

        public TemplateService(FormsWebAppDbContext context, UserManager<ApplicationUser> userManager, LuceneSearchService luceneSearchService)
        { 
            _context = context;
            _luceneSearchService = luceneSearchService;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        }

        public async Task<List<string>> SearchTagsAsync(string query)
        {
            return await _context.Tags
                .Where(t => t.TName.StartsWith(query))
                .Select(t => t.TName)
                .ToListAsync();
        }

        public async Task SetTagsForTemplateAsync(Template template, List<string>? tagNames)
        {
            if (tagNames == null || !tagNames.Any())
                return;
            var existingTags = await _context.Tags
                .Where(t => tagNames.Contains(t.TName))
                .ToListAsync();

            var newTags = tagNames.Except(existingTags.Select(t => t.TName))
                .Select(name => new Tag { TName = name })
                .ToList();

            if (newTags.Any())
            {
                _context.Tags.AddRange(newTags);
                await _context.SaveChangesAsync();
            }

            template.TemplateTags = existingTags.Concat(newTags)
                .Select( tag => new TemplateTag { Tag = tag })
                .ToList();
        }

        public void CallReIndex()
        {
            var templates =_context.Templates
                .Include(t => t.Author)
                .Where(t => t.Visibility == TemplateVisibility.Public)
                .ToList();
                _luceneSearchService.Reindex(templates);
        }

        public async Task<List<Template>> GetLatestTemplatesAsync(int skip, int take)
        {
            return await _context.Templates
                .OrderByDescending(t => t.DateCreated)
                .Skip(skip)
                .Take(take)
                .Include(t => t.Likes)
                .Include(t => t.Comments)
                .Include(t => t.Author)
                .Where(t => t.Visibility == TemplateVisibility.Public)
                .ToListAsync();
        }
        public async Task<List<Template>> GetTemplatesByTagAsync(string tagName, int skip, int take)
        {
            return await _context.Templates
                .Where(t => t.Visibility == TemplateVisibility.Public &&
                            t.TemplateTags.Any(tt => tt.Tag.TName == tagName))
                .OrderByDescending(t => t.DateCreated)
                .Skip(skip)
                .Take(take)
                .Include(t => t.Likes)
                .Include(t => t.Comments)
                .Include(t => t.Author)
                .ToListAsync();
        }

        public async Task<List<Tag>> GetPopularTagsAsync(int count = 20)
        {
            return await _context.Tags
                .OrderByDescending(t => t.TemplateTags.Count)
                .Take(count)
                .ToListAsync();
        }


        public int GetAnswerCount(int templateId)
        {
            return _context.Answers.Count(a => a.TemplateId == templateId);
        }


        public async Task<List<Template>> GetUserTemplatesAsync(string userId)
        {
            return await _context.Templates
                .Where(t => t.AuthorId == userId)
                .Include(t => t.Likes)
                .Include(t => t.Comments)
                .OrderByDescending(t => t.DateCreated)
                .AsNoTracking()
                .ToListAsync();

        }

        public async Task<int> CreateTemplateAsync(Template template, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            template.AuthorId = userId;
            template.Author = user;

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();
            return template.Id;
        }


        public async Task<Template?> GetTemplateByIdAsync(int id, string userId, bool isAdmin)
        {
            return await _context.Templates
                .Include(t => t.TemplateTags)
                .Include(t => t.Author)
                .Include(t => t.Likes)
                .Include(t => t.AllowedUsers)
                .Include(t => t.Comments)
                .Where(t => t.Id == id)
                .Where(t => isAdmin ||
                       (t.Visibility == TemplateVisibility.Public ||
                       (t.Visibility == TemplateVisibility.Restricted && t.AllowedUsers.Any(u => u.Id == userId)) ||
                       (t.Visibility == TemplateVisibility.Private && t.AuthorId == userId)))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SubmitResponseAsync(Answer answer, string userId)
        {
            if (answer == null) return false;

            answer.UserId = userId;
            answer.SubmittedAt = DateTime.UtcNow;

            var template = await _context.Templates.FindAsync(answer.TemplateId);
            if (template == null)
            {
                Console.WriteLine("Error: Template not found.");
                return false;
            }
            answer.Template = template;

            answer.CustomString1State = !string.IsNullOrWhiteSpace(answer.CustomString1Answer);
            answer.CustomString2State = !string.IsNullOrWhiteSpace(answer.CustomString2Answer);
            answer.CustomString3State = !string.IsNullOrWhiteSpace(answer.CustomString3Answer);
            answer.CustomString4State = !string.IsNullOrWhiteSpace(answer.CustomString4Answer);

            answer.CustomMultiLine1State = !string.IsNullOrWhiteSpace(answer.CustomMultiLine1Answer);
            answer.CustomMultiLine2State = !string.IsNullOrWhiteSpace(answer.CustomMultiLine2Answer);
            answer.CustomMultiLine3State = !string.IsNullOrWhiteSpace(answer.CustomMultiLine3Answer);
            answer.CustomMultiLine4State = !string.IsNullOrWhiteSpace(answer.CustomMultiLine4Answer);

            answer.CustomInt1State = answer.CustomInt1Answer.HasValue && answer.CustomInt1Answer.Value != 0;
            answer.CustomInt2State = answer.CustomInt2Answer.HasValue && answer.CustomInt2Answer.Value != 0;
            answer.CustomInt3State = answer.CustomInt3Answer.HasValue && answer.CustomInt3Answer.Value != 0;
            answer.CustomInt4State = answer.CustomInt4Answer.HasValue && answer.CustomInt4Answer.Value != 0;

            Console.WriteLine($"Saving response for UserId={answer.UserId}, TemplateId={answer.TemplateId}");

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikeTemplateAsync(int templateId, string userId)
        {
            var (template, user) = await GetTemplateAndUserAsync(templateId, userId);

            if (template == null || user == null) return false;
            if (template.AuthorId == userId) return false; 

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.TemplateId == templateId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
            }
            else
            {
                Console.WriteLine($"[INFO] Adding like by {userId} on template {templateId}.");
                _context.Likes.Add(new Like { TemplateId = templateId, UserId = userId, Template = template, User = user });
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<int> GetLikeCountAsync(int templateId)
        {
            return await _context.Likes.Where(l => l.TemplateId == templateId).CountAsync();
        }

        private async Task<(Template template, ApplicationUser user)> GetTemplateAndUserAsync(int templateId, string userId)
        {
            var template = await _context.Templates.FindAsync(templateId);
            var user = await _context.Users.FindAsync(userId);
            return (template, user);
        }

        public async Task<bool> AddCommentAsync(int templateId, string userId, string content)
        {
            var (template, user) = await GetTemplateAndUserAsync(templateId, userId);
            if (template == null || user == null) return false;

            _context.Comments.Add(new Comment
            {
                TemplateId = templateId,
                UserId = userId,
                Content = content,
                User = user,
                Template = template
            });
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Comment>> GetCommentsAsync(int templateId)
        {
            return await _context.Comments
                .Where( c=> c.TemplateId == templateId)
                .OrderBy( c => c.Date)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Template?> GetTemplateForEditAsync(int templateId)
        {
            return await _context.Templates
                .FirstOrDefaultAsync(t => t.Id == templateId);

        }
        public async Task<bool> UpdateTemplateAsync(int templateId, Template updatedTemplate)
        {
            var template = await _context.Templates
                .FirstOrDefaultAsync(t => t.Id == templateId);

            if (template == null) return false;
            template.Title = updatedTemplate.Title ?? template.Title;
            template.Title = updatedTemplate.Title ?? template.Title;
            template.Description = updatedTemplate.Description ?? template.Description;

            // Custom String Fields
            template.CustomString1State = template.CustomString1State || updatedTemplate.CustomString1State;
            template.CustomString1Question = updatedTemplate.CustomString1Question ?? template.CustomString1Question;

            template.CustomString2State = template.CustomString2State || updatedTemplate.CustomString2State;
            template.CustomString2Question = updatedTemplate.CustomString2Question ?? template.CustomString2Question;

            template.CustomString3State = template.CustomString3State || updatedTemplate.CustomString3State;
            template.CustomString3Question = updatedTemplate.CustomString3Question ?? template.CustomString3Question;

            template.CustomString4State = template.CustomString4State || updatedTemplate.CustomString4State;
            template.CustomString4Question = updatedTemplate.CustomString4Question ?? template.CustomString4Question;

            // Custom Multi-line Fields
            template.CustomMultiLine1State = template.CustomMultiLine1State || updatedTemplate.CustomMultiLine1State;
            template.CustomMultiLine1Question = updatedTemplate.CustomMultiLine1Question ?? template.CustomMultiLine1Question;

            template.CustomMultiLine2State = template.CustomMultiLine2State || updatedTemplate.CustomMultiLine2State;
            template.CustomMultiLine2Question = updatedTemplate.CustomMultiLine2Question ?? template.CustomMultiLine2Question;

            template.CustomMultiLine3State = template.CustomMultiLine3State || updatedTemplate.CustomMultiLine3State;
            template.CustomMultiLine3Question = updatedTemplate.CustomMultiLine3Question ?? template.CustomMultiLine3Question;

            template.CustomMultiLine4State = template.CustomMultiLine4State || updatedTemplate.CustomMultiLine4State;
            template.CustomMultiLine4Question = updatedTemplate.CustomMultiLine4Question ?? template.CustomMultiLine4Question;

            // Custom Integer Fields
            template.CustomInt1State = template.CustomInt1State || updatedTemplate.CustomInt1State;
            template.CustomInt1Question = updatedTemplate.CustomInt1Question ?? template.CustomInt1Question;

            template.CustomInt2State = template.CustomInt2State || updatedTemplate.CustomInt2State;
            template.CustomInt2Question = updatedTemplate.CustomInt2Question ?? template.CustomInt2Question;

            template.CustomInt3State = template.CustomInt3State || updatedTemplate.CustomInt3State;
            template.CustomInt3Question = updatedTemplate.CustomInt3Question ?? template.CustomInt3Question;

            template.CustomInt4State = template.CustomInt4State || updatedTemplate.CustomInt4State;
            template.CustomInt4Question = updatedTemplate.CustomInt4Question ?? template.CustomInt4Question;

           
            _context.Templates.Update(template);

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Template successfully updated.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving template: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTemplateAsync(int templateId)
        {
            var template = await _context.Templates
                .FirstOrDefaultAsync(t => t.Id == templateId);


            if (template == null)
                return false;
            template.AllowedUsers = null;

            var answers = await _context.Answers
                .Where(a => a.TemplateId == templateId)
                .ToListAsync(); 

            _context.Answers.RemoveRange(answers);
            _context.Likes.RemoveRange(template.Likes);
            _context.Comments.RemoveRange(template.Comments);
            _context.TemplateTags.RemoveRange(template.TemplateTags);

            _context.Templates.Remove(template);

            await _context.SaveChangesAsync();
            return true;
        }

    }

}

