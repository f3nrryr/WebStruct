using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Enums;

namespace UsersRoles.Services.Interfaces
{
    public interface IUsersService
    {
        // Базовые CRUD операции
        Task<ServiceResult<UserServiceModel>> CreateUserAsync(UserCreateServiceModel userDto);
        Task<ServiceResult> UpdateUserAsync(string userId, UserUpdateServiceModel userDto);
        Task<ServiceResult> DeleteUserAsync(string userId);
        Task<UserServiceModel> GetUserByIdAsync(string userId);
        Task<List<UserServiceModel>> GetAllUsersAsync();

        // Управление ролями
        Task<ServiceResult> AssignRolesToUserAsync(string userId, List<string> roles);
        Task<List<string>> GetUserRolesAsync(string userId);

        // Управление паролями
        Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<ServiceResult> AdminChangePasswordAsync(string userId, string newPassword);

        // Email подтверждение
        Task<ServiceResult> SendEmailConfirmationAsync(string userId, string callbackUrl);
        Task<ServiceResult> ConfirmEmailAsync(string userId, string token);
        Task<ServiceResult> SendPasswordResetAsync(string email, string callbackUrl);
        Task<ServiceResult> ResetPasswordAsync(string email, string token, string newPassword);

        // Проверка полномочий
        Task<bool> UserHasPermissionAsync(string userId, PermissionEnum permission);
        Task<List<PermissionServiceModel>> GetUserPermissionsAsync(string userId);

        // Активация/деактивация
        Task<ServiceResult> ActivateUserAsync(string userId);
        Task<ServiceResult> DeactivateUserAsync(string userId);
    }
}
