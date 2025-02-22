using FormsWebApplication.Interface;
using FormsWebApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FormsWebApplication.Controllers
{
    //    [Authorize(Roles = "Admin")]
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var userRoles = await _userService.GetUserRolesAsync();
            ViewBag.UserRoles = userRoles;
            return View(users);
        }


        public async Task<IActionResult> Block(string id)
        {
            bool success = await _userService.BlockUserAsync(id);
            if (!success) return NotFound();
            return Redirect("/Admin/Index");
        }

        public async Task<IActionResult> Unblock(string id)
        {
            bool success = await _userService.UnblockUserAsync(id);
            if (!success) return NotFound();
            return Redirect("/Admin/Index");
        }


        public async Task<IActionResult> Delete(string id)
        {
            bool success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound();
            return Redirect("/Admin/Index");
        }

        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            bool success = await _userService.PromoteToAdminAsync(id);
            if (!success) return NotFound();
            return Redirect("/Admin/Index");
        }

        public async Task<IActionResult> RemoveAdmin(string id)
        {
            bool success = await _userService.RemoveAdminAccessAsync(id);
            if (!success) return NotFound();
            return RedirectToAction("Index", "Admin");
        }
    }
}
