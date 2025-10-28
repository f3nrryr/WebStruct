using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebStruct.Contracts.Users
{
    public class UserResponse
    {
        public int Id { get; set; }

        [MaxLength(32)]
        public string Login { get; set; }

        [MaxLength(255)]
        public string FullName { get; set; }

        public int[] Roles { get; set; }
    }
}
