using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebStruct.Contracts.Roles
{
    public class RoleResponse
    {
        public int Id { get; set; }

        [MinLength(3)]
        [MaxLength(255)]
        public string Name { get; set; }
        public int[] Rights { get; set; }
    }
}
