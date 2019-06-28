using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Core.Models
{
    public class UserRoleModel
    {
        public int UserRoleId { get; set; }
        [Required(ErrorMessage = "User is required")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string RoleId { get; set; }

        public UserModel User { get; set; }
        public RoleModel Role { get; set; }
    }
}
