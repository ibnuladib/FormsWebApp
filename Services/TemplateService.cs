using FormsWebApplication.Data;
using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace FormsWebApplication.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly FormsWebAppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TemplateService(FormsWebAppDbContext context, UserManager<ApplicationUser> userManager)
        { 
            _context = context;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        }
        public async Task<List<Template>> GetLatestTemplatesAsync(int skip, int take)
        {
            return await _context.Templates
                .OrderByDescending(t => t.DateCreated) 
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
        public async Task<List<Template>> GetUserTemplatesAsync(string userId)
        {
            return await _context.Templates
                .Where(t => t.AuthorId == userId)
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


        public async Task<Template?> GetTemplateByIdAsync(int id)
        {
            return await _context.Templates
                .Include(t => t.TemplateTags)
                .Include(t => t.Author)
                .Include(t => t.Likes)
                .FirstOrDefaultAsync(t => t.Id == id);


        }

        public async Task<bool> SubmitResponseAsync(Answer answer, string userId)
        {
            if (answer == null) return false;

            // Assign metadata
            answer.UserId = userId;
            answer.SubmittedAt = DateTime.UtcNow;

            // Fetch the template
            var template = await _context.Templates.FindAsync(answer.TemplateId);
            if (template == null)
            {
                Console.WriteLine("Error: Template not found.");
                return false;
            }
            answer.Template = template;

            // Ensure answers are stored correctly
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

            //answer.CustomCheckbox1State = answer.CustomCheckbox1Answer ?? false;
            //answer.CustomCheckbox2State = answer.CustomCheckbox2Answer ?? false;
            //answer.CustomCheckbox3State = answer.CustomCheckbox3Answer ?? false;
            //answer.CustomCheckbox4State = answer.CustomCheckbox4Answer ?? false;

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
                .OrderByDescending( c => c.Date)
                .Include(c => c.User)
                .ToListAsync();
        }

        async Task<Template?> ITemplateService.GetTemplateForEditAsync(int templateId)
        {
            return await _context.Templates
                .Where(t => t.Id == templateId)
                .Select(t => new Template
                {
                    Id = t.Id,
                    Title = t.Title,
                    Author = t.Author,
                    AuthorId = t.AuthorId,
                    CustomString1State = t.CustomString1State,
                    CustomString1Question = t.CustomString1Question,
                    CustomString2State = t.CustomString2State,
                    CustomString2Question = t.CustomString2Question,
                    CustomString3State = t.CustomString3State,
                    CustomString3Question = t.CustomString3Question,
                    CustomString4State = t.CustomString4State,
                    CustomString4Question = t.CustomString4Question,

                    CustomMultiLine1State = t.CustomMultiLine1State,
                    CustomMultiLine1Question = t.CustomMultiLine1Question,
                    CustomMultiLine2State = t.CustomMultiLine2State,
                    CustomMultiLine2Question = t.CustomMultiLine2Question,
                    CustomMultiLine3State = t.CustomMultiLine3State,
                    CustomMultiLine3Question = t.CustomMultiLine3Question,
                    CustomMultiLine4State = t.CustomMultiLine4State,
                    CustomMultiLine4Question = t.CustomMultiLine4Question,

                    CustomInt1State = t.CustomInt1State,
                    CustomInt1Question = t.CustomInt1Question,
                    CustomInt2State = t.CustomInt2State,
                    CustomInt2Question = t.CustomInt2Question,
                    CustomInt3State = t.CustomInt3State,
                    CustomInt3Question = t.CustomInt3Question,
                    CustomInt4State = t.CustomInt4State,
                    CustomInt4Question = t.CustomInt4Question,

                    CustomCheckbox1State = t.CustomCheckbox1State,
                    CustomCheckbox1Question = t.CustomCheckbox1Question,
                    CustomCheckbox2State = t.CustomCheckbox2State,
                    CustomCheckbox2Question = t.CustomCheckbox2Question,
                    CustomCheckbox3State = t.CustomCheckbox3State,
                    CustomCheckbox3Question = t.CustomCheckbox3Question,
                    CustomCheckbox4State = t.CustomCheckbox4State,
                    CustomCheckbox4Question = t.CustomCheckbox4Question

                })
                .FirstOrDefaultAsync();
        }

        async Task<bool> ITemplateService.UpdateTemplateAsync(int templateId, Template updatedTemplate, string userId)
        {
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == templateId && t.AuthorId == userId);

            if(template == null) return false;

            template.Title = updatedTemplate.Title;
            template.Author = updatedTemplate.Author;
            template.AuthorId = updatedTemplate.AuthorId;

            template.CustomString1State = updatedTemplate.CustomString1State;
            template.CustomString1Question = updatedTemplate.CustomString1Question;
            template.CustomString2State = updatedTemplate.CustomString2State;
            template.CustomString2Question = updatedTemplate.CustomString2Question;
            template.CustomString3State = updatedTemplate.CustomString3State;
            template.CustomString3Question = updatedTemplate.CustomString3Question;
            template.CustomString4State = updatedTemplate.CustomString4State;
            template.CustomString4Question = updatedTemplate.CustomString4Question;

            template.CustomMultiLine1State = updatedTemplate.CustomMultiLine1State;
            template.CustomMultiLine1Question = updatedTemplate.CustomMultiLine1Question;
            template.CustomMultiLine2State = updatedTemplate.CustomMultiLine2State;
            template.CustomMultiLine2Question = updatedTemplate.CustomMultiLine2Question;
            template.CustomMultiLine3State = updatedTemplate.CustomMultiLine3State;
            template.CustomMultiLine3Question = updatedTemplate.CustomMultiLine3Question;
            template.CustomMultiLine4State = updatedTemplate.CustomMultiLine4State;
            template.CustomMultiLine4Question = updatedTemplate.CustomMultiLine4Question;

            template.CustomInt1State = updatedTemplate.CustomInt1State;
            template.CustomInt1Question = updatedTemplate.CustomInt1Question;
            template.CustomInt2State = updatedTemplate.CustomInt2State;
            template.CustomInt2Question = updatedTemplate.CustomInt2Question;
            template.CustomInt3State = updatedTemplate.CustomInt3State;
            template.CustomInt3Question = updatedTemplate.CustomInt3Question;
            template.CustomInt4State = updatedTemplate.CustomInt4State;
            template.CustomInt4Question = updatedTemplate.CustomInt4Question;

            template.CustomCheckbox1State = updatedTemplate.CustomCheckbox1State;
            template.CustomCheckbox1Question = updatedTemplate.CustomCheckbox1Question;
            template.CustomCheckbox2State = updatedTemplate.CustomCheckbox2State;
            template.CustomCheckbox2Question = updatedTemplate.CustomCheckbox2Question;
            template.CustomCheckbox3State = updatedTemplate.CustomCheckbox3State;
            template.CustomCheckbox3Question = updatedTemplate.CustomCheckbox3Question;
            template.CustomCheckbox4State = updatedTemplate.CustomCheckbox4State;
            template.CustomCheckbox4Question = updatedTemplate.CustomCheckbox4Question;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
