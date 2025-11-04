using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Enums;

namespace UsersRoles.Services.Interfaces
{
    public interface IRolesService
    {
        // Базовые CRUD операции для ролей
        Task<ServiceResult<RoleServiceModel>> CreateRoleAsync(RoleCreateServiceModel roleDto);
        Task<ServiceResult> UpdateRoleAsync(string roleId, RoleUpdateServiceModel roleDto);
        Task<ServiceResult> DeleteRoleAsync(string roleId);
        Task<RoleServiceModel> GetRoleByIdAsync(string roleId);
        Task<List<RoleServiceModel>> GetAllRolesAsync();

        // Управление полномочиями ролей
        Task<ServiceResult> AddPermissionToRoleAsync(string roleId, PermissionEnum permission, Guid grantedBy);
        Task<ServiceResult> RemovePermissionFromRoleAsync(string roleId, PermissionEnum permission);
        Task<ServiceResult> SetRolePermissionsAsync(string roleId, List<PermissionEnum> permissions, Guid grantedBy);
        Task<List<PermissionEnum>> GetRolePermissionsAsync(string roleId);

        // Управление ролями пользователей
        Task<ServiceResult> AssignRolesToUserAsync(string userId, List<string> roles);
        Task<List<string>> GetUserRolesAsync(string userId);

        // Проверка полномочий пользователей
        Task<bool> UserHasPermissionAsync(string userId, PermissionEnum permission);
        Task<List<PermissionEnum>> GetUserPermissionsAsync(string userId);

        // Утилиты
        Task<bool> RoleExistsAsync(string roleName);
    }
}
