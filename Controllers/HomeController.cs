using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Diagnostics;

namespace FormsWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITemplateService _templateService;

        public HomeController(ITemplateService templateService)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }


        public async Task<IActionResult> Index()
        {
            var templates = await _templateService.GetLatestTemplatesAsync(0, 20);
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
