using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Services.DTOs;

namespace UsersRoles.Services.Interfaces
{
    public interface IPermissionsService
    {
        // Базовые CRUD операции для полномочий
        Task<ServiceResult<PermissionServiceModel>> CreatePermissionAsync(PermissionCreateServiceModel permissionDto);
        Task<ServiceResult> DeletePermissionAsync(int permissionId);
        Task<PermissionServiceModel> GetPermissionByIdAsync(int permissionId);
        Task<PermissionServiceModel> GetPermissionByNameAsync(string name);
        Task<List<PermissionServiceModel>> GetAllPermissionsAsync();

        // Управление связями ролей и полномочий
        Task<ServiceResult> AddPermissionToRoleAsync(string roleId, int permissionId, Guid grantedBy);
        Task<ServiceResult> RemovePermissionFromRoleAsync(string roleId, int permissionId);
        Task<ServiceResult> SetRolePermissionsAsync(string roleId, List<int> permissionIds, Guid grantedBy);
        Task<List<PermissionServiceModel>> GetRolePermissionsAsync(string roleId);

        // Получение полномочий пользователей
        Task<List<PermissionServiceModel>> GetUserPermissionsAsync(string userId);
        Task<bool> UserHasPermissionAsync(string userId, int permissionId);
        Task<bool> UserHasPermissionAsync(string userId, string permissionName);

        // Групповые операции
        Task<ServiceResult> AddPermissionsToRoleAsync(string roleId, List<int> permissionIds, Guid grantedBy);
        Task<Dictionary<string, List<PermissionServiceModel>>> GetPermissionsByRolesAsync(List<string> roleIds);
    }
}
