using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersRoles.DAL.CodeFirst
{
    public class WebStructPermission
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<WebStructRolePermission> RolePermissions { get; set; } = new List<WebStructRolePermission>();

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
