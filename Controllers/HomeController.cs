using FormsWebApplication.Data;
using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using FormsWebApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace FormsWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly LuceneSearchService _luceneSearchService;
        private readonly FormsWebAppDbContext _context;

        public HomeController(ITemplateService templateService, LuceneSearchService luceneSearchService, FormsWebAppDbContext context)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _luceneSearchService = luceneSearchService;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var templates = await _templateService.GetLatestTemplatesAsync(0, 20);

            foreach (var template in templates)
            {
                template.AnswerCount = _templateService.GetAnswerCount(template.Id);
            }

            return View(templates);
        }


        [HttpGet]
        public async Task<IActionResult> LoadMoreTemplates(int page)
        {
            int pageSize = 20;
            var templates = await _templateService.GetLatestTemplatesAsync(page * pageSize, pageSize);

            if(templates == null)
            {
                return NoContent();
            }
            return PartialView("_TemplateListPartial",templates);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new List<Template>());
            var templates = _context.Templates
                .Include(t => t.Author)
                .Where(t => t.Visibility == TemplateVisibility.Public)
                .ToList();
           // _luceneSearchService.Reindex(templates);
            var results = _luceneSearchService.SearchTemplates(query);
            return View("SearchResults", results);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Reindex()
        {
            var templates = _context.Templates.Include(t => t.Author).ToList(); 
            _luceneSearchService.Reindex(templates);
            return RedirectToAction("Index");
        }




    }
}
