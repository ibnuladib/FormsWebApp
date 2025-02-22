using FormsWebApplication.Data;
using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FormsWebApplication.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly FormsWebAppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnswerService(FormsWebAppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Answer?> GetTemplateAnswersAsync(int answerId, string userId, bool isAdmin)
        {
            return await _context.Answers
                .Where( a => a.Id == answerId && (a.UserId == userId || isAdmin))
                .Include( a => a.User)
                .Include( a=> a.Template)
                .FirstOrDefaultAsync(); 
        }

        public async Task<List<Answer>> GetAnswerByTemplateIdAsync(int templateId, string userId, bool isAdmin)
        {
            return await _context.Answers
                .Where( a => a.TemplateId == templateId && (a.Template.AuthorId == userId || isAdmin))
                .Include( a => a.User)
                .Include(a => a.Template)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
        }

        public async Task<List<Answer>> GetUserAnswersAsync(string userId)
        {
            return await _context.Answers
                .Where(a => a.UserId == userId)
                .Include(a => a.Template)
                .OrderByDescending( a => a.SubmittedAt)
                .ToListAsync();
        }
        public async Task<bool> DeleteAnswerAsync(int id, string userId, bool isAdmin)
        {
            var answer = await _context.Answers
                .Include(a => a.Template)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null || (!isAdmin && answer.Template.AuthorId != userId))
                return false;

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, List<int>>> GetCustomIntAnswersAsync(int templateId, string userId, bool isAdmin)
        {
            var template = await _context.Templates
                .Where(t => t.Id == templateId)
                .Select(t => new
                {
                    t.AuthorId,
                    t.CustomInt1State,
                    t.CustomInt2State,
                    t.CustomInt3State,
                    t.CustomInt4State,
                })
                .FirstOrDefaultAsync();

            if (template == null || (!isAdmin && template.AuthorId != userId))
            {
                return null; // Unauthorized or template not found
            }

            var answers = await _context.Answers
                .Where(a => a.TemplateId == templateId)
                .Select(a => new
                {
                    a.CustomInt1Answer,
                    a.CustomInt2Answer,
                    a.CustomInt3Answer,
                    a.CustomInt4Answer
                })
                .ToListAsync();

            var intAnswers = new Dictionary<string, List<int>>();

            if (template.CustomInt1State)
                intAnswers["CustomInt1"] = answers.Where(a => a.CustomInt1Answer.HasValue).Select(a => a.CustomInt1Answer.Value).ToList();

            if (template.CustomInt2State)
                intAnswers["CustomInt2"] = answers.Where(a => a.CustomInt2Answer.HasValue).Select(a => a.CustomInt2Answer.Value).ToList();

            if (template.CustomInt3State)
                intAnswers["CustomInt3"] = answers.Where(a => a.CustomInt3Answer.HasValue).Select(a => a.CustomInt3Answer.Value).ToList();

            if (template.CustomInt4State)
                intAnswers["CustomInt4"] = answers.Where(a => a.CustomInt4Answer.HasValue).Select(a => a.CustomInt4Answer.Value).ToList();

            return intAnswers;
        }

        public double CalculateAverage(IEnumerable<int> numbers)
        {
            return numbers.Any() ? numbers.Average() : 0;
        }
    }
}
