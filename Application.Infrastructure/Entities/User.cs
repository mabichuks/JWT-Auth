using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Infrastructure.Entities
{
    public class User
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public ICollection<SocialLogin> SocialLogins { get; set; } = new HashSet<SocialLogin>();

    }
}
