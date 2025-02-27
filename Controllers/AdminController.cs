using FormsWebApplication.Interface;
using FormsWebApplication.Models;
using FormsWebApplication.Services;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormsWebApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet("SearchUsers")]
        [Route("/Admin/SearchUsers")]
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty");

            var users = await _userManager.Users
                .Where(u => u.FirstName.Contains(query) || u.LastName.Contains(query) || u.Email.Contains(query))
                .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email })
                .Take(10)
                .ToListAsync();

            return Ok(users);
        }


        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var userRoles = await _userService.GetUserRolesAsync();
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Block(string id)
        {
            bool success = await _userService.BlockUserAsync(id);
            if (!success) return NotFound();
            return Redirect("/Admin/Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unblock(string id)
        {
            bool success = await _userService.UnblockUserAsync(id);
            if (!success) return NotFound();
            return Redirect("/Admin/Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            bool success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound();
            return Redirect("/Admin/Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            bool success = await _userService.PromoteToAdminAsync(id);
            if (!success) return NotFound();
            return Redirect("/Admin/Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            bool success = await _userService.RemoveAdminAccessAsync(id);
            if (!success) return NotFound();
            return RedirectToAction("Index", "Admin");
        }

    }
}
