using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FormsWebApplication.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TemplateController(ITemplateService templateService, UserManager<ApplicationUser> userManager)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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

        [HttpPost]
        public async Task<IActionResult> SubmitResponse(Answer model)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors)
            //                                  .Select(e => e.ErrorMessage)
            //                                  .ToList();
            //    foreach (var error in errors)
            //    {
            //        Console.WriteLine($"Model Error: {error}");
            //    }

            //    var template = await _templateService.GetTemplateByIdAsync(model.TemplateId);
            //    if (template == null)
            //    {
            //        return NotFound();
            //    }
            //    return View("Response", template);
            //}

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
        public ActionResult Details(int id)
        {
            return View();
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
            return CreatedAtAction(nameof(GetTemplateById), new { id = templateId }, template);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            if (template == null) return NotFound();

            return Ok(template);
        }

        // GET: TemplateController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TemplateController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TemplateController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TemplateController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
