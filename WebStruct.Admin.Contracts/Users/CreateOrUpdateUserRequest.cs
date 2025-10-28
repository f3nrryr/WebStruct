using System;
using System.Collections.Generic;
using System.Text;

namespace WebStruct.Admin.Contracts.Users
{
    public class CreateOrUpdateUserRequest
    {
        public string Login { get; set; }
        public string FullName { get; set; }
        public int[] Roles { get; set; }
    }
}
