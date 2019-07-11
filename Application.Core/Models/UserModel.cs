using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Core.Models
{
    public class UserModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }

        public RoleModel[] Roles { get; set; }

        public List<SocialLoginModel> SocialLogins { get; set; } = new List<SocialLoginModel>();

    }
}
