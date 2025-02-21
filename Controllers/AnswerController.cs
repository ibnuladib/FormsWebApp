using FormsWebApplication.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FormsWebApplication.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FormsWebApplication.Controllers
{
    [Authorize]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnswerController(IAnswerService answerService, UserManager<ApplicationUser> userManager)
        {
            _answerService = answerService;
            _userManager = userManager;
        }

        // Helper function to get the current user ID and admin status
        private async Task<(string userId, bool isAdmin)> GetCurrentUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return (user.Id, isAdmin);
        }

        // GET: User's submitted answers
        public async Task<IActionResult> Index()
        {
            var (userId, _) = await GetCurrentUserInfo();
            var answers = await _answerService.GetUserAnswersAsync(userId);
            return View(answers);
        }

        // GET: Answers for a specific template (only visible to author or admin)
        public async Task<IActionResult> TemplateAnswers(int templateId)
        {
            var (userId, isAdmin) = await GetCurrentUserInfo();
            var answers = await _answerService.GetAnswerByTemplateIdAsync(templateId, userId, isAdmin);

            return View("TemplateAnswers", answers);
        }

        // GET: Answer Details
        public async Task<IActionResult> Details(int id)
        {
            var (userId, isAdmin) = await GetCurrentUserInfo();
            var answer = await _answerService.GetTemplateAnswersAsync(id, userId, isAdmin);

            if (answer == null)
                return Forbid();

            return View(answer);
        }


        // POST: Delete Answer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (userId, isAdmin) = await GetCurrentUserInfo();
 
            var success = await _answerService.DeleteAnswerAsync(id, userId, isAdmin);

            if (!success)
                return Forbid();

            return RedirectToAction("Index", "Answer");

        }

        public async Task<IActionResult> Average(int templateId)
        {
            var (userId, isAdmin) = await GetCurrentUserInfo();
            var intAnswers = await _answerService.GetCustomIntAnswersAsync(templateId, userId, isAdmin);

            if (intAnswers == null)
            {
                return Forbid();
            }

            var averages = intAnswers
                .Where(kv => kv.Value.Any())
                .ToDictionary(kv => kv.Key, kv => _answerService.CalculateAverage(kv.Value));

            return View(averages);
        }
    }
}
