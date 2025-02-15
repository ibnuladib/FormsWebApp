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
                .FirstOrDefaultAsync(t => t.Id == id);


        }

        //public async Task<Template?> GetTemplateForResponseAsync(int templateId)
        //{
        //    return await _context.Templates
        //        .Where(t => t.Id  == templateId)
        //        .Select(t => new Template
        //        {
        //            Id = t.Id,
        //            Title = t.Title,
        //            TemplateTags = t.TemplateTags,
        //            CustomCheckbox1Question
        //        })
        //}

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

            // Debugging output
            Console.WriteLine($"Saving response for UserId={answer.UserId}, TemplateId={answer.TemplateId}");

            // Save response
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return true;
        }







    }
}
