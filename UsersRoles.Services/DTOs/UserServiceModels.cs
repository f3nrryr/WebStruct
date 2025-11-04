using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersRoles.Services.DTOs
{
    public class UserServiceModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; } = new();
        public Guid CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public Guid? LastUpdatedBy { get; set; }
    }

    public class UserCreateServiceModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } = new();
        public Guid CreatedBy { get; set; }
    }

    public class UserUpdateServiceModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public Guid UpdatedBy { get; set; }
    }

    public class ChangePasswordServiceModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class AdminChangePasswordServiceModel
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
