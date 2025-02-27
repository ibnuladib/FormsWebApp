using System.Security.Claims;
using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using FormsWebApplication.Services;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FormsWebApplication.Controllers
{
    [Authorize]
    public class TemplateController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TemplateController> _logger;

        public TemplateController(
            ITemplateService templateService,
            UserManager<ApplicationUser> userManager,
            ILogger<TemplateController> logger
            )
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger;
        }

        private string CurrentUserId => _userManager.GetUserId(User)
            ?? throw new UnauthorizedAccessException("User not authenticated");

        private async Task<Template> GetTemplateOrThrow(int id)
        {
            bool isAdmin = await IsAdminAsync(CurrentUserId);
            return await _templateService.GetTemplateByIdAsync(id, CurrentUserId, isAdmin)
                ?? throw new KeyNotFoundException($"Template {id} not found");
        }

        private IActionResult HandleError(Exception ex, string message = "An error occurred")
        {
            _logger.LogError(ex, message);
            return ex switch
            {
                UnauthorizedAccessException => Unauthorized(ex.Message),
                KeyNotFoundException => NotFound(ex.Message),
                _ => StatusCode(500, message)
            };
        }

        private async Task<bool> IsAuthorized(Template template, string userId)
        {
            if (template.AuthorId == userId) return true;
            return await IsAdminAsync(userId);
        }

        private async Task<bool> IsAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _templateService.GetUserTemplatesAsync(CurrentUserId));
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to load templates");
            }
        }



        [HttpGet("SearchTags")]
        public async Task<IActionResult> SearchTags(string query)
        {
            if(string.IsNullOrEmpty(query))
                return BadRequest("Query cannot be empty");

            var tags = await _templateService.SearchTagsAsync(query);
            return Ok(tags); 
        }

        [HttpGet]
        public new async Task<IActionResult> Response(int id)
        {
            try
            {
                return View(await GetTemplateOrThrow(id));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitResponse(Answer model)
        {
            try
            {
                model.UserId = CurrentUserId;
                return await _templateService.SubmitResponseAsync(model, CurrentUserId)
                    ? RedirectToAction("Index", "Home")
                    : BadRequest("Failed to submit response");
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Response submission failed");
            }
        }


        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                return View(await GetTemplateOrThrow(id));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        public ActionResult Create() => View();

        [Authorize]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [FromForm] Template template,
            [FromForm] string? selectedUserIds,
            [FromForm] string tagNames)
        {
            try
            {
                var userId = CurrentUserId;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                template.AuthorId = userId;
                ApplyVisibilitySettings(template, selectedUserIds);

                var tagList = string.IsNullOrEmpty(tagNames) ? new List<string>(): JsonConvert.DeserializeObject<List<string>>(tagNames);

                await _templateService.SetTagsForTemplateAsync(template, tagList);

                var templateId = await _templateService.CreateTemplateAsync(template, userId);
                return RedirectToAction("Details", new { id = templateId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Template creation failed");
                return (ActionResult)HandleError(ex, "Template creation failed");
            }
        }

        private void ApplyVisibilitySettings(Template template, string? selectedUserIds)
        {
            if (template.Visibility == TemplateVisibility.Restricted && !string.IsNullOrEmpty(selectedUserIds))
            {
                var userIds = selectedUserIds.Split(',').Distinct().ToList();
                var existingUsers = _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToList();

                template.AllowedUsers = existingUsers;
            }
            else
            {
                template.AllowedUsers = null;
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            try
            {
                return Ok(await GetTemplateOrThrow(id));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                return View(await _templateService.GetTemplateForEditAsync(id)
                    ?? throw new KeyNotFoundException("Template not found"));
            }
            catch (Exception ex)
            {
                return (ActionResult)HandleError(ex);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [FromForm] Template updatedTemplate)
        {
            try
            {
                var existing = await GetTemplateOrThrow(id);
                if (!await IsAuthorized(existing, CurrentUserId))
                {
                    return Forbid();
                }
                var updateSuccessful = await _templateService.UpdateTemplateAsync(id, updatedTemplate);


                    // Only reindex if update was successful
                    _templateService.CallReIndex();
                    return RedirectToAction("Details", new { id });

            }
            catch (Exception ex)
            {
                return (ActionResult)HandleError(ex, "Template update failed");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var template = await GetTemplateOrThrow(id);

                if (!await IsAuthorized(template, CurrentUserId))
                {
                    return Forbid();
                }

                return await _templateService.DeleteTemplateAsync(id)
                    ? Redirect("Template/Index")
                    : BadRequest("Delete failed");
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Template deletion failed");
            }
        }

        [Authorize]
        [HttpPost("Template/Like/{templateId}")]
        public async Task<IActionResult> LikeTemplate(int templateId)
        {
            try
            {
                var success = await _templateService.LikeTemplateAsync(templateId, CurrentUserId);
                return Ok(new
                {
                    success,
                    likeCount = success ? await _templateService.GetLikeCountAsync(templateId) : 0
                });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Like operation failed");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(int templateId, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                    return BadRequest("Comment content required");

                return Ok(new
                {
                    success = await _templateService.AddCommentAsync(templateId, CurrentUserId, content),
                    message = "Comment added successfully"
                });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Comment addition failed");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int templateId)
        {
            try
            {
                var comments = await _templateService.GetCommentsAsync(templateId);
                return Ok(comments.Select(c => new {
                    c.Id,
                    c.Content,
                    c.Date,
                    c.User.UserName
                }));
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to load comments");
            }
        }
    }
}