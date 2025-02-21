using FormsWebApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormsWebApplication.Interface
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<bool> BlockUserAsync(string userId);
        Task<bool> UnblockUserAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> PromoteToAdminAsync(string userId);
        Task<bool> RemoveAdminAccessAsync(string userId);
    }
}

