using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Repositories.DTOs;
using UsersRoles.Repositories.Interfaces;
using UsersRoles.Repositories.Repositories;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Enums;
using UsersRoles.Services.Interfaces;

namespace UsersRoles.Services.Implementations
{
    public class RolesService : IRolesService
    {
        private readonly IRolesRepository _roleRepository;
        private readonly IPermissionsRepository _permissionRepository;
        private readonly IUsersRepository _userRepository;

        public RolesService(
            IRolesRepository roleRepository,
            IPermissionsRepository permissionRepository,
            IUsersRepository userRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<RoleServiceModel>> CreateRoleAsync(RoleCreateServiceModel roleDto)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(roleDto.Name))
                    return ServiceResult<RoleServiceModel>.Failure("Role name is required");

                if (await _roleRepository.ExistsAsync(roleDto.Name))
                    return ServiceResult<RoleServiceModel>.Failure("Role with this name already exists");

                // Создание роли
                var roleModel = new RoleCreateModel
                {
                    Name = roleDto.Name,
                    Description = roleDto.Description,
                    CreatedBy = roleDto.CreatedBy
                };

                var role = await _roleRepository.CreateAsync(roleModel);

                // Добавление полномочий к роли
                if (roleDto.Permissions != null && roleDto.Permissions.Any())
                {
                    foreach (var permission in roleDto.Permissions)
                    {
                        var permissionName = permission.GetName();
                        var permissionEntity = await _permissionRepository.GetByNameAsync(permissionName);
                        if (permissionEntity != null)
                        {
                            await _permissionRepository.AddPermissionToRoleAsync(role.Id, permissionEntity.Id, roleDto.CreatedBy);
                        }
                    }
                }

                var result = await GetRoleByIdAsync(role.Id);
                return ServiceResult<RoleServiceModel>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<RoleServiceModel>.Failure($"Error creating role: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateRoleAsync(string roleId, RoleUpdateServiceModel roleDto)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    return ServiceResult.Failure("Role not found");

                var updateModel = new RoleUpdateModel
                {
                    Id = roleId,
                    Name = roleDto.Name,
                    Description = roleDto.Description,
                    UpdatedBy = roleDto.UpdatedBy
                };

                await _roleRepository.UpdateAsync(updateModel);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error updating role: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    return ServiceResult.Failure("Role not found");

                // Проверка, есть ли пользователи в этой роли
                var usersInRole = await _userRepository.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Any())
                    return ServiceResult.Failure("Cannot delete role that has users assigned");

                await _roleRepository.DeleteAsync(roleId);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error deleting role: {ex.Message}");
            }
        }

        public async Task<ServiceResult> AddPermissionToRoleAsync(string roleId, PermissionEnum permission, Guid grantedBy)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    return ServiceResult.Failure("Role not found");

                var permissionName = permission.GetName();
                var permissionEntity = await _permissionRepository.GetByNameAsync(permissionName);
                if (permissionEntity == null)
                    return ServiceResult.Failure("Permission not found");

                await _permissionRepository.AddPermissionToRoleAsync(roleId, permissionEntity.Id, grantedBy);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error adding permission to role: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RemovePermissionFromRoleAsync(string roleId, PermissionEnum permission)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    return ServiceResult.Failure("Role not found");

                var permissionName = permission.GetName();
                var permissionEntity = await _permissionRepository.GetByNameAsync(permissionName);
                if (permissionEntity == null)
                    return ServiceResult.Failure("Permission not found");

                await _permissionRepository.RemovePermissionFromRoleAsync(roleId, permissionEntity.Id);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error removing permission from role: {ex.Message}");
            }
        }

        public async Task<ServiceResult> SetRolePermissionsAsync(string roleId, List<PermissionEnum> permissions, Guid grantedBy)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    return ServiceResult.Failure("Role not found");

                var permissionIds = new List<int>();
                foreach (var permission in permissions)
                {
                    var permissionName = permission.GetName();
                    var permissionEntity = await _permissionRepository.GetByNameAsync(permissionName);
                    if (permissionEntity != null)
                    {
                        permissionIds.Add(permissionEntity.Id);
                    }
                }

                await _permissionRepository.SetRolePermissionsAsync(roleId, permissionIds, grantedBy);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error setting role permissions: {ex.Message}");
            }
        }

        public async Task<List<PermissionEnum>> GetRolePermissionsAsync(string roleId)
        {
            var permissions = await _permissionRepository.GetRolePermissionsAsync(roleId);
            var result = new List<PermissionEnum>();

            foreach (var permission in permissions)
            {
                if (Enum.TryParse<PermissionEnum>(permission.Name.Replace(".", ""), out var permissionEnum))
                {
                    result.Add(permissionEnum);
                }
            }

            return result;
        }

        public async Task<bool> UserHasPermissionAsync(string userId, PermissionEnum permission)
        {
            var permissionName = permission.GetName();
            return await _permissionRepository.UserHasPermissionAsync(userId, permissionName);
        }

        public async Task<List<PermissionEnum>> GetUserPermissionsAsync(string userId)
        {
            var permissions = await _permissionRepository.GetUserPermissionsAsync(userId);
            var result = new List<PermissionEnum>();

            foreach (var permission in permissions)
            {
                if (Enum.TryParse<PermissionEnum>(permission.Name.Replace(".", ""), out var permissionEnum))
                {
                    result.Add(permissionEnum);
                }
            }

            return result;
        }

        public async Task<ServiceResult> AssignRolesToUserAsync(string userId, List<string> roles)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                // Проверяем существование всех ролей
                foreach (var role in roles)
                {
                    if (!await _roleRepository.ExistsAsync(role))
                        return ServiceResult.Failure($"Role '{role}' does not exist");
                }

                await _userRepository.SetUserRolesAsync(userId, roles);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error assigning roles: {ex.Message}");
            }
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            return await _userRepository.GetUserRolesAsync(userId);
        }

        public async Task<RoleServiceModel> GetRoleByIdAsync(string roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) return null;

            var permissions = await GetRolePermissionsAsync(roleId);

            return new RoleServiceModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = permissions,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                LastUpdatedAt = role.UpdatedAt,
                LastUpdatedBy = role.LastUpdatedBy
            };
        }

        public async Task<List<RoleServiceModel>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            var result = new List<RoleServiceModel>();

            foreach (var role in roles)
            {
                var permissions = await GetRolePermissionsAsync(role.Id);

                result.Add(new RoleServiceModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    Permissions = permissions,
                    CreatedAt = role.CreatedAt,
                    CreatedBy = role.CreatedBy,
                    LastUpdatedAt = role.UpdatedAt,
                    LastUpdatedBy = role.LastUpdatedBy
                });
            }

            return result;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleRepository.ExistsAsync(roleName);
        }
    }
}
