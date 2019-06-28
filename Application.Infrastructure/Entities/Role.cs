using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Infrastructure.Entities
{
    public class Role
    {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
