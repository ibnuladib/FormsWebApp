using FormsWebApplication.Models;
using FormsWebApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace FormsWebApplication.Controllers
{
    [Route("api/salesforce")]
    [ApiController]
    public class SalesforceController : ControllerBase
    {
        private readonly SalesforceService _salesforceService;

        public SalesforceController(SalesforceService salesforceService)
        {
            _salesforceService = salesforceService;
        }

        // 🔹 API: Create Account
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Account name is required.");
            }

            try
            {
                var accountId = await _salesforceService.CreateAccountAsync(request.Name);
                return Ok(new { Message = "Account created successfully!", AccountId = accountId });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating account: {ex.Message}");
            }
        }

        // 🔹 API: Create Contact and Link to Account
        [HttpPost("create-contact")]
        public async Task<IActionResult> CreateContact([FromBody] CreateContactRequest request)
        {
            if (string.IsNullOrEmpty(request.AccountId) || string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName))
            {
                return BadRequest("Account ID, First Name, and Last Name are required.");
            }

            try
            {
                await _salesforceService.CreateContactAsync(request.AccountId, request.FirstName, request.LastName, request.Email);
                return Ok(new { Message = "Contact created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating contact: {ex.Message}");
            }
        }
    }
}
