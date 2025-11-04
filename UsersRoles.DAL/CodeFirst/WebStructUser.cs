using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersRoles.DAL.CodeFirst
{
    public class WebStructUser : IdentityUser
    {
        public string FullName { get; set; }

        /// <summary>
        /// Возможность деактивации учётки юзера.
        /// </summary>
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid? LastUpdatedBy { get; set; }
    }
}
