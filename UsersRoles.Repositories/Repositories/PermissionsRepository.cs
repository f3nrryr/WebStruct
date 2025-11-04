using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersRoles.DAL.CodeFirst;
using UsersRoles.Repositories.DTOs;
using UsersRoles.Repositories.Interfaces;

namespace Infrastructure.Repositories;

public class PermissionsRepository : IPermissionsRepository
{
    private readonly WebStructContext _context;
    private readonly UserManager<WebStructUser> _userManager;

    public PermissionsRepository(WebStructContext context, UserManager<WebStructUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<RoleModel>> GetRolesWithPermissionAsync(int permissionId)
    {
        var roles = await _context.RolePermissions
            .Where(rp => rp.PermissionId == permissionId)
            .Include(rp => rp.Role)
            .Select(rp => rp.Role)
            .ToListAsync();

        return roles.Select(role => new RoleModel
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            CreatedBy = role.CreatedBy,
            LastUpdatedBy = role.LastUpdatedBy,
            UpdatedAt = role.LastUpdatedAt
        }).ToList();
    }

    public async Task<List<PermissionModel>> GetUserPermissionsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<PermissionModel>();

        var userRoles = await _userManager.GetRolesAsync(user);

        var permissions = await _context.RolePermissions
                          .Where(rp => userRoles.Contains(rp.Role.Name))
                          .Include(rp => rp.Permission)
                          .Include(rp => rp.Role)
                          .Select(rp => rp.Permission)
                          .Distinct()
                          .ToListAsync();

        return permissions.Select(MapToModel).ToList();
    }

    public async Task<bool> UserHasPermissionAsync(string userId, string permissionName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var userRoles = await _userManager.GetRolesAsync(user);

        return await _context.RolePermissions
               .Include(rp => rp.Role)
               .Include(rp => rp.Permission)
               .AnyAsync(rp =>
                   userRoles.Contains(rp.Role.Name) &&
                   rp.Permission.Name == permissionName);
    }

    public async Task<bool> UserHasPermissionAsync(string userId, int permissionId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var userRoles = await _userManager.GetRolesAsync(user);

        return await _context.RolePermissions
            .Include(rp => rp.Role)
            .AnyAsync(rp =>
                userRoles.Contains(rp.Role.Name) &&
                rp.PermissionId == permissionId);
    }

    public async Task<List<PermissionModel>> GetRolePermissionsAsync(string roleId)
    {
        var permissions = await _context.RolePermissions
                          .Where(rp => rp.RoleId == roleId)
                          .Include(rp => rp.Permission)
                          .Select(rp => rp.Permission)
                          .ToListAsync();

        return permissions.Select(MapToModel).ToList();
    }

    public async Task<bool> RoleHasPermissionAsync(string roleId, int permissionId)
    {
        return await _context.RolePermissions
                     .AnyAsync
                     (rp => rp.RoleId == roleId 
                     && rp.PermissionId == permissionId);
    }

    public async Task AddPermissionToRoleAsync(string roleId, int permissionId, Guid grantedBy)
    {
        var existing = await _context.RolePermissions
                       .FirstOrDefaultAsync
                       (rp => rp.RoleId == roleId 
                       && rp.PermissionId == permissionId);

        if (existing == null)
        {
            _context.RolePermissions.Add(new WebStructRolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                GrantedBy = grantedBy,
                GrantedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemovePermissionFromRoleAsync(string roleId, int permissionId)
    {
        var rolePermission = await _context.RolePermissions
                             .FirstOrDefaultAsync
                             (rp => rp.RoleId == roleId 
                             && rp.PermissionId == permissionId);

        if (rolePermission != null)
        {
            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddPermissionsToRoleAsync(string roleId, List<int> permissionIds, Guid grantedBy)
    {
        var existingPermissions = await _context.RolePermissions
                                  .Where(rp => rp.RoleId == roleId)
                                  .Select(rp => rp.PermissionId)
                                  .ToListAsync();

        var newPermissions = permissionIds.Except(existingPermissions).ToList();

        foreach (var permissionId in newPermissions)
        {
            _context.RolePermissions.Add(new WebStructRolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                GrantedBy = grantedBy,
                GrantedAt = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task SetRolePermissionsAsync(string roleId, List<int> permissionIds, Guid grantedBy)
    {
        var currentPermissions = _context.RolePermissions.Where(rp => rp.RoleId == roleId);
        _context.RolePermissions.RemoveRange(currentPermissions);

        foreach (var permissionId in permissionIds)
        {
            _context.RolePermissions.Add(new WebStructRolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                GrantedBy = grantedBy,
                GrantedAt = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<PermissionModel> GetByIdAsync(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        return MapToModel(permission);
    }

    public async Task<PermissionModel> GetByNameAsync(string name)
    {
        var permission = await _context.Permissions
                         .FirstOrDefaultAsync
                         (p => p.Name == name);
        return MapToModel(permission);
    }

    public async Task<List<PermissionModel>> GetAllAsync()
    {
        var permissions = await _context.Permissions.ToListAsync();
        return permissions.Select(MapToModel).ToList();
    }

    public async Task<PermissionModel> CreateAsync(PermissionCreateModel model)
    {
        var permission = new WebStructPermission
        {
            Name = model.Name,
            Description = model.Description,
            CreatedAt = DateTime.Now,
            CreatedBy = model.CreatedBy
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        return MapToModel(permission);
    }

    public async Task DeleteAsync(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission != null)
        {
            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
        }
    }

    private PermissionModel MapToModel(WebStructPermission permission)
    {
        if (permission == null) return null;

        return new PermissionModel
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            CreatedAt = permission.CreatedAt,
            CreatedBy = permission.CreatedBy
        };
    }
}