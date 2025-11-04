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
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? LastUpdatedBy { get; set; }
    }
}
