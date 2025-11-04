using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Services.Enums;

namespace UsersRoles.Services.DTOs
{
    public class RoleServiceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionEnum> Permissions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public Guid? LastUpdatedBy { get; set; }
    }

    public class RoleCreateServiceModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionEnum> Permissions { get; set; } = new();
        public Guid CreatedBy { get; set; }
    }

    public class RoleUpdateServiceModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionEnum> Permissions { get; set; } = new();
        public Guid UpdatedBy { get; set; }
    }
}
