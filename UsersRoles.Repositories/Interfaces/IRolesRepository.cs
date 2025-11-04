using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Repositories.DTOs;

namespace UsersRoles.Repositories.Interfaces
{
    public interface IRolesRepository
    {
        Task<RoleModel> GetByIdAsync(string id);
        Task<RoleModel> GetByNameAsync(string name);
        Task<List<RoleModel>> GetAllAsync();
        Task<RoleModel> CreateAsync(RoleCreateModel role);
        Task UpdateAsync(RoleUpdateModel role);
        Task DeleteAsync(string id);
        Task<bool> ExistsAsync(string roleName);
    }
}
