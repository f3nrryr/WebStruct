using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace UsersRoles.DAL.CodeFirst
{
    public class WebStructRolePermission
    {
        public int Id { get; set; }

        public string RoleId { get; set; }
        public WebStructRole Role { get; set; }

        public int PermissionId { get; set; }
        public WebStructPermission Permission { get; set; }

        public DateTime GrantedAt { get; set; }
        public Guid GrantedBy { get; set; }
    }
}
