using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Admin.Contracts.Roles
{
    public class CreateOrUpdateRoleRequest
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public int[] Rights { get; set; }
    }
}
