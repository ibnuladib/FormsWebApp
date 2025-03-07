using Microsoft.AspNetCore.Mvc;
using FormsWebApplication.Services;
using FormsWebApplication.Models;

namespace FormsWebApplication.Controllers
{
    public class SalesforceUIController : Controller
    {
        private readonly SalesforceService _salesforceService;

        public SalesforceUIController(SalesforceService salesforceService)
        {
            _salesforceService = salesforceService;
        }

        // Show the Create Account Form
        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        // Handle Form Submission
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _salesforceService.CreateAccountAsync(model.Name, model.Phone, model.Website);
                ViewBag.SuccessMessage = "Account created successfully!";
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating account: " + ex.Message);
                return View(model);
            }
        }
    }
}
