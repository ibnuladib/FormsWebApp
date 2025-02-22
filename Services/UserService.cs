
    using FormsWebApplication.Data;
    using FormsWebApplication.Interface;
    using FormsWebApplication.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

namespace FormsWebApplication.Services
{
    public class UserService : IUserService
    {
        private readonly FormsWebAppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(FormsWebAppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> BlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Block indefinitely
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UnblockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.LockoutEnd = null; // Unblock user
            await _userManager.UpdateAsync(user);
            return true;
        }


        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            await _userManager.DeleteAsync(user);
            return true;
        }

        public async Task<bool> PromoteToAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            if (await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
            }
            await _userManager.AddToRoleAsync(user, "Admin");
            return true;
        }

        public async Task<bool> RemoveAdminAccessAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            await _userManager.RemoveFromRoleAsync(user, "Admin");
            return true;
        }


        public async Task<Dictionary<string, List<string>>> GetUserRolesAsync()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, List<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            return userRoles;
        }
    }

}
