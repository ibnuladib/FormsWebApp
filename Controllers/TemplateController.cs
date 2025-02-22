using System.Security.Claims;
using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using FormsWebApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FormsWebApplication.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TemplateController> _logger;
        private readonly LuceneSearchService _luceneSearchService;

        public TemplateController(ITemplateService templateService, UserManager<ApplicationUser> userManager, ILogger<TemplateController> logger, LuceneSearchService luceneSearchService)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger;
            _luceneSearchService = luceneSearchService;
        }


        // GET: TemplateController
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var templates = await _templateService.GetUserTemplatesAsync(userId);
            return View(templates);
        }

        [HttpGet]
        public async Task<IActionResult> Response(int id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            else
            {
                return View(template);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitResponse(Answer model)
        {
            string? userId = _userManager.GetUserId(User);
            model.UserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(model.UserId))
            {
                return Unauthorized();
            }

            bool success = await _templateService.SubmitResponseAsync(model, userId);
            if (!success)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Home");
        }





        // GET: TemplateController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);

            if (template == null)
            {
                return NotFound();
            }

            return View(template);
        }


        // GET: TemplateController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TemplateController/Create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] Template template)
        {
            string? userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            int templateId = await _templateService.CreateTemplateAsync(template, userId);
            _luceneSearchService.IndexTemplates(new List<Template> { template });
          // return CreatedAtAction(nameof(GetTemplateById), new { id = templateId }, template);
            return RedirectToAction("Details", new { id = templateId });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            if (template == null) return NotFound();

            return Ok(template);
        }

        // GET: TemplateController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var template = await _templateService.GetTemplateForEditAsync(id);
            if (template == null) return NotFound();
            return View(template);
        }

        // POST: TemplateController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [FromForm] Template updatedTemplate)
        {
            string? userId = _userManager?.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var existingTemplate = await _templateService.GetTemplateForEditAsync(id);
            if (existingTemplate == null) return NotFound();

            updatedTemplate.AuthorId = userId;
            updatedTemplate.Author = existingTemplate.Author;
            _logger.LogInformation("Updated Template Data: {@updatedTemplate}", updatedTemplate);

            bool success = await _templateService.UpdateTemplateAsync(id, updatedTemplate, userId);
            if (!success) return BadRequest("Could not update Template");
            _luceneSearchService.IndexTemplates(new List<Template> { existingTemplate });
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            bool success = await _templateService.DeleteTemplateAsync(id);

            if (!success)
                return NotFound();
            if (template == null) return NotFound();
            _luceneSearchService.IndexTemplates(new List<Template> { template });
            return Redirect("/Template/Index");
        }


        [HttpPost]
        [Route("Template/Like/{templateId}")]
        public async Task<IActionResult> LikeTemplate(int templateId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _templateService.LikeTemplateAsync(templateId, userId);

            return Ok(new { success = result, likeCount = await _templateService.GetLikeCountAsync(templateId) });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int templateId, string content)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) { return Unauthorized(); }
            if (string.IsNullOrEmpty(content)) { return BadRequest("empty comment"); }

            var result = await _templateService.AddCommentAsync(templateId, userId, content);
            return Ok(new { success = result, message = "Comment Added Sucessfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int templateId)
        {
            var comments = await _templateService.GetCommentsAsync(templateId);
            return Ok(comments.Select(c => new
            {
                c.Id,
                c.Content,
                c.Date,
                UserName = c.User.UserName
            }));
        }
    }
}

