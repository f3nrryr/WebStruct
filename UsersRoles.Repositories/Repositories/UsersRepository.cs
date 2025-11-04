using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersRoles.DAL.CodeFirst;
using UsersRoles.Repositories.DTOs;
using UsersRoles.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly WebStructContext _context;
    private readonly UserManager<WebStructUser> _userManager;
    private readonly RoleManager<WebStructRole> _roleManager;

    public UsersRepository(
        WebStructContext context,
        UserManager<WebStructUser> userManager,
        RoleManager<WebStructRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<string>();

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
            throw new Exception($"Add to role failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task RemoveFromRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
            throw new Exception($"Remove from role failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task SetUserRolesAsync(string userId, List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var currentRoles = await _userManager.GetRolesAsync(user);

        // Удаляем старые роли
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            throw new Exception($"Remove roles failed: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");

        // Добавляем новые роли
        var addResult = await _userManager.AddToRolesAsync(user, roles);
        if (!addResult.Succeeded)
            throw new Exception($"Add roles failed: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
    }

    public async Task<List<UserModel>> GetUsersInRoleAsync(string roleName)
    {
        var users = await _userManager.GetUsersInRoleAsync(roleName);
        return users.Select(MapToModel).ToList();
    }

    public async Task<UserModel> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return MapToModel(user);
    }

    public async Task<UserModel> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return MapToModel(user);
    }

    public async Task<List<UserModel>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return users.Select(MapToModel).ToList();
    }

    public async Task<UserModel> CreateAsync(UserCreateModel model, string password)
    {
        var user = new WebStructUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName,
            CreatedAt = DateTime.Now,
            IsActive = true,
            CreatedBy = model.CreatedBy
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return MapToModel(user);
    }

    public async Task UpdateAsync(UserUpdateModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) throw new Exception("User not found");

        user.UserName = model.UserName;
        user.Email = model.Email;
        user.FullName = model.FullName;
        user.IsActive = model.IsActive;
        user.LastUpdatedAt = DateTime.Now;
        user.LastUpdatedBy = model.UpdatedBy;

        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
    }

    public async Task<bool> CheckPasswordAsync(string userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            throw new Exception($"Password change failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task ResetPasswordAsync(string userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            throw new Exception($"Password reset failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            throw new Exception($"Email confirmation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> IsEmailConfirmedAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.EmailConfirmed ?? false;
    }

    private UserModel MapToModel(WebStructUser user)
    {
        if (user == null) return null;

        return new UserModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.LastUpdatedAt,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            UpdatedBy = user.LastUpdatedBy,
            CreatedBy = user.CreatedBy
        };
    }
}