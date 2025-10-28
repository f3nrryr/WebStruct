using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersRoles.DAL.CodeFirst
{
    public class WebStructRole : IdentityRole
    {
        public string Description { get; set; }
        public ICollection<WebStructRolePermission> RolePermissions { get; set; }
    }
}
