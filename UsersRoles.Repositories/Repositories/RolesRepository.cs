using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.DAL.CodeFirst;
using UsersRoles.Repositories.DTOs;
using UsersRoles.Repositories.Interfaces;

namespace UsersRoles.Repositories.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly WebStructContext _context;
        private readonly RoleManager<WebStructRole> _roleManager;

        public RolesRepository(WebStructContext context, RoleManager<WebStructRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<RoleModel> GetByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return MapToModel(role);
        }

        public async Task<RoleModel> GetByNameAsync(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            return MapToModel(role);
        }

        public async Task<List<RoleModel>> GetAllAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(MapToModel).ToList();
        }

        public async Task<RoleModel> CreateAsync(RoleCreateModel model)
        {
            var role = new WebStructRole
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                CreatedBy = model.CreatedBy
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                throw new Exception($"Role creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return MapToModel(role);
        }

        public async Task UpdateAsync(RoleUpdateModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null) throw new Exception("Role not found");

            role.Name = model.Name;
            role.Description = model.Description;
            role.LastUpdatedBy = model.UpdatedBy;
            role.LastUpdatedAt = DateTime.Now;

            await _roleManager.UpdateAsync(role);
        }

        public async Task DeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
        }

        public async Task<bool> ExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        private RoleModel MapToModel(WebStructRole role)
        {
            if (role == null) return null;

            return new RoleModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                LastUpdatedBy = role.LastUpdatedBy,
                UpdatedAt = role.LastUpdatedAt
            };
        }
    }
}
