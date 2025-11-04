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
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IRolesRepository _roleRepository;
        private readonly IPermissionsRepository _permissionRepository;

        public UsersService(
            IUsersRepository userRepository,
            IRolesRepository roleRepository,
            IPermissionsRepository permissionRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<ServiceResult<UserServiceModel>> CreateUserAsync(UserCreateServiceModel userDto)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(userDto.Email))
                    return ServiceResult<UserServiceModel>.Failure("Email is required");

                if (string.IsNullOrWhiteSpace(userDto.Password))
                    return ServiceResult<UserServiceModel>.Failure("Password is required");

                // Проверка существования пользователя
                var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                if (existingUser != null)
                    return ServiceResult<UserServiceModel>.Failure("User with this email already exists");

                // Создание пользователя
                var userCreateModel = new UserCreateModel
                {
                    UserName = userDto.UserName ?? userDto.Email,
                    Email = userDto.Email,
                    FullName = userDto.FullName,
                    Password = userDto.Password,
                    CreatedBy = userDto.CreatedBy
                };

                var user = await _userRepository.CreateAsync(userCreateModel, userDto.Password);

                // Назначение ролей
                if (userDto.Roles != null && userDto.Roles.Any())
                {
                    foreach (var role in userDto.Roles)
                    {
                        if (await _roleRepository.ExistsAsync(role))
                        {
                            await _userRepository.AddToRoleAsync(user.Id, role);
                        }
                    }
                }

                var result = await GetUserByIdAsync(user.Id);
                return ServiceResult<UserServiceModel>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserServiceModel>.Failure($"Error creating user: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateUserAsync(string userId, UserUpdateServiceModel userDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                var updateModel = new UserUpdateModel
                {
                    Id = userId,
                    UserName = user.UserName, // Сохраняем текущее имя пользователя
                    Email = userDto.Email,
                    FullName = userDto.FullName,
                    IsActive = userDto.IsActive,
                    UpdatedBy = userDto.UpdatedBy
                };

                await _userRepository.UpdateAsync(updateModel);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error updating user: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                await _userRepository.DeleteAsync(userId);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                    return ServiceResult.Failure("Password must be at least 6 characters long");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                // Проверка текущего пароля
                var isValidPassword = await _userRepository.CheckPasswordAsync(userId, currentPassword);
                if (!isValidPassword)
                    return ServiceResult.Failure("Current password is incorrect");

                await _userRepository.ChangePasswordAsync(userId, currentPassword, newPassword);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error changing password: {ex.Message}");
            }
        }

        public async Task<ServiceResult> AdminChangePasswordAsync(string userId, string newPassword)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                    return ServiceResult.Failure("Password must be at least 6 characters long");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                await _userRepository.ResetPasswordAsync(userId, newPassword);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error changing password: {ex.Message}");
            }
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

        public async Task<UserServiceModel> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userRepository.GetUserRolesAsync(userId);
            var permissions = await _permissionRepository.GetUserPermissionsAsync(userId);

            return new UserServiceModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                LastUpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles
            };
        }

        public async Task<List<UserServiceModel>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var result = new List<UserServiceModel>();

            foreach (var user in users)
            {
                var roles = await _userRepository.GetUserRolesAsync(user.Id);
                var permissions = await _permissionRepository.GetUserPermissionsAsync(user.Id);

                result.Add(new UserServiceModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    CreatedAt = user.CreatedAt,
                    LastUpdatedAt = user.UpdatedAt,
                    CreatedBy = user.CreatedBy,
                    LastUpdatedBy = user.UpdatedBy,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles
                });
            }

            return result;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            return await _userRepository.GetUserRolesAsync(userId);
        }

        public async Task<bool> UserHasPermissionAsync(string userId, PermissionEnum permission)
        {
            var permissionName = permission.GetName();
            return await _permissionRepository.UserHasPermissionAsync(userId, permissionName);
        }

        public async Task<List<PermissionServiceModel>> GetUserPermissionsAsync(string userId)
        {
            var permissions = await _permissionRepository.GetUserPermissionsAsync(userId);
            return permissions.Select(p => new PermissionServiceModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<ServiceResult> SendEmailConfirmationAsync(string userId, string callbackUrl)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                if (user.EmailConfirmed)
                    return ServiceResult.Failure("Email is already confirmed");

                var token = await _userRepository.GenerateEmailConfirmationTokenAsync(userId);

                // Здесь должна быть реальная логика отправки email
                // Например: await _emailService.SendEmailConfirmationAsync(user.Email, callbackUrl + "?token=" + token);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error sending email confirmation: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                if (user.EmailConfirmed)
                    return ServiceResult.Failure("Email is already confirmed");

                await _userRepository.ConfirmEmailAsync(userId, token);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error confirming email: {ex.Message}");
            }
        }

        public async Task<ServiceResult> SendPasswordResetAsync(string email, string callbackUrl)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                var token = await _userRepository.GeneratePasswordResetTokenAsync(user.Id);

                // Здесь должна быть реальная логика отправки email
                // Например: await _emailService.SendPasswordResetAsync(email, callbackUrl + "?token=" + token);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error sending password reset: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                // Валидация пароля
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                    return ServiceResult.Failure("Password must be at least 6 characters long");

                // Используем встроенный метод для сброса пароля
                await _userRepository.ResetPasswordAsync(user.Id, newPassword);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error resetting password: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                var updateModel = new UserUpdateModel
                {
                    Id = userId,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = true
                    //UpdatedBy тут не используется.
                };

                await _userRepository.UpdateAsync(updateModel);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error activating user: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ServiceResult.Failure("User not found");

                var updateModel = new UserUpdateModel
                {
                    Id = userId,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = false
                    //UpdatedBy тут не используется.
                };

                await _userRepository.UpdateAsync(updateModel);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error deactivating user: {ex.Message}");
            }
        }

    }
}
