using UsersRoles.Repositories.DTOs;

namespace UsersRoles.Repositories.Interfaces
{
    public interface IPermissionsRepository
    {
        Task<PermissionModel> GetByIdAsync(int id);
        Task<PermissionModel> GetByNameAsync(string name);
        Task<List<PermissionModel>> GetAllAsync();
        Task<PermissionModel> CreateAsync(PermissionCreateModel permission);
        Task<PermissionModel> UpdateAsync(PermissionUpdateModel permission);
        Task DeleteAsync(int id);

        // Управление связями
        Task AddPermissionToRoleAsync(string roleId, int permissionId, Guid grantedBy);
        Task RemovePermissionFromRoleAsync(string roleId, int permissionId);
        Task<bool> RoleHasPermissionAsync(string roleId, int permissionId);
        Task<List<PermissionModel>> GetRolePermissionsAsync(string roleId);
        Task<List<PermissionModel>> GetUserPermissionsAsync(string userId);
        Task<bool> UserHasPermissionAsync(string userId, int permissionId);
        Task<bool> UserHasPermissionAsync(string userId, string permissionName);

        // Групповые операции
        Task AddPermissionsToRoleAsync(string roleId, List<int> permissionIds, Guid grantedBy);
        Task SetRolePermissionsAsync(string roleId, List<int> permissionIds, Guid grantedBy);
    }
}
