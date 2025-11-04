using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Repositories.DTOs;

namespace UsersRoles.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<UserModel> GetByIdAsync(string id);
        Task<UserModel> GetByEmailAsync(string email);
        Task<List<UserModel>> GetAllAsync();
        Task<UserModel> CreateAsync(UserCreateModel user, string password);
        Task UpdateAsync(UserUpdateModel user);
        Task DeleteAsync(string id);
        Task<bool> CheckPasswordAsync(string userId, string password);
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task ResetPasswordAsync(string userId, string newPassword);
        Task AddToRoleAsync(string userId, string role);
        Task RemoveFromRoleAsync(string userId, string role);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task ConfirmEmailAsync(string userId, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(string userId);
        Task<string> GeneratePasswordResetTokenAsync(string userId);
        Task<bool> IsEmailConfirmedAsync(string userId);
        Task<List<UserModel>> GetUsersInRoleAsync(string roleName);
        Task SetUserRolesAsync(string userId, List<string> roles);
    }
}
