using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Repositories.DTOs;
using UsersRoles.Repositories.Interfaces;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Interfaces;

namespace UsersRoles.Services.Implementations
{
    public class PermissionsService : IPermissionsService
    {
        private readonly IPermissionsRepository _permissionRepository;

        public PermissionsService(IPermissionsRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<ServiceResult<PermissionServiceModel>> CreatePermissionAsync(PermissionCreateServiceModel permissionDto)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(permissionDto.Name))
                    return ServiceResult<PermissionServiceModel>.Failure("Permission name is required");

                // Проверка существования
                var existing = await _permissionRepository.GetByNameAsync(permissionDto.Name);
                if (existing != null)
                    return ServiceResult<PermissionServiceModel>.Failure("Permission with this name already exists");

                // Создание
                var createModel = new PermissionCreateModel
                {
                    Name = permissionDto.Name,
                    Description = permissionDto.Description,
                    CreatedBy = permissionDto.CreatedBy
                };

                var permission = await _permissionRepository.CreateAsync(createModel);
                var result = MapToServiceModel(permission);

                return ServiceResult<PermissionServiceModel>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<PermissionServiceModel>.Failure($"Error creating permission: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeletePermissionAsync(int permissionId)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId);
                if (permission == null)
                    return ServiceResult.Failure("Permission not found");

                // Проверка, используется ли полномочие в каких-либо ролях
                var rolesWithPermission = await _permissionRepository.GetRolesWithPermissionAsync(permissionId);
                if (rolesWithPermission.Any())
                    return ServiceResult.Failure("Cannot delete permission that is assigned to roles");

                await _permissionRepository.DeleteAsync(permissionId);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error deleting permission: {ex.Message}");
            }
        }

        public async Task<PermissionServiceModel> GetPermissionByIdAsync(int permissionId)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            return MapToServiceModel(permission);
        }

        public async Task<PermissionServiceModel> GetPermissionByNameAsync(string name)
        {
            var permission = await _permissionRepository.GetByNameAsync(name);
            return MapToServiceModel(permission);
        }

        public async Task<List<PermissionServiceModel>> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            return permissions.Select(MapToServiceModel).ToList();
        }

        public async Task<ServiceResult> AddPermissionToRoleAsync(string roleId, int permissionId, Guid grantedBy)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId);
                if (permission == null)
                    return ServiceResult.Failure("Permission not found");

                await _permissionRepository.AddPermissionToRoleAsync(roleId, permissionId, grantedBy);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error adding permission to role: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RemovePermissionFromRoleAsync(string roleId, int permissionId)
        {
            try
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId);
                if (permission == null)
                    return ServiceResult.Failure("Permission not found");

                await _permissionRepository.RemovePermissionFromRoleAsync(roleId, permissionId);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error removing permission from role: {ex.Message}");
            }
        }

        public async Task<ServiceResult> SetRolePermissionsAsync(string roleId, List<int> permissionIds, Guid grantedBy)
        {
            try
            {
                // Проверяем существование всех полномочий
                foreach (var permissionId in permissionIds)
                {
                    var permission = await _permissionRepository.GetByIdAsync(permissionId);
                    if (permission == null)
                        return ServiceResult.Failure($"Permission with ID {permissionId} not found");
                }

                await _permissionRepository.SetRolePermissionsAsync(roleId, permissionIds, grantedBy);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error setting role permissions: {ex.Message}");
            }
        }

        public async Task<List<PermissionServiceModel>> GetRolePermissionsAsync(string roleId)
        {
            var permissions = await _permissionRepository.GetRolePermissionsAsync(roleId);
            return permissions.Select(MapToServiceModel).ToList();
        }

        public async Task<List<PermissionServiceModel>> GetUserPermissionsAsync(string userId)
        {
            var permissions = await _permissionRepository.GetUserPermissionsAsync(userId);
            return permissions.Select(MapToServiceModel).ToList();
        }

        public async Task<bool> UserHasPermissionAsync(string userId, int permissionId)
        {
            return await _permissionRepository.UserHasPermissionAsync(userId, permissionId);
        }

        public async Task<bool> UserHasPermissionAsync(string userId, string permissionName)
        {
            return await _permissionRepository.UserHasPermissionAsync(userId, permissionName);
        }

        public async Task<ServiceResult> AddPermissionsToRoleAsync(string roleId, List<int> permissionIds, Guid grantedBy)
        {
            try
            {
                // Проверяем существование всех полномочий
                foreach (var permissionId in permissionIds)
                {
                    var permission = await _permissionRepository.GetByIdAsync(permissionId);
                    if (permission == null)
                        return ServiceResult.Failure($"Permission with ID {permissionId} not found");
                }

                await _permissionRepository.AddPermissionsToRoleAsync(roleId, permissionIds, grantedBy);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error adding permissions to role: {ex.Message}");
            }
        }

        public async Task<Dictionary<string, List<PermissionServiceModel>>> GetPermissionsByRolesAsync(List<string> roleIds)
        {
            var result = new Dictionary<string, List<PermissionServiceModel>>();

            foreach (var roleId in roleIds)
            {
                var permissions = await GetRolePermissionsAsync(roleId);
                result[roleId] = permissions;
            }

            return result;
        }

        private PermissionServiceModel MapToServiceModel(PermissionModel model)
        {
            if (model == null) return null;

            return new PermissionServiceModel
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                CreatedBy = model.CreatedBy
            };
        }
    }
}
