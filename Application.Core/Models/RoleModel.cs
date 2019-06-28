using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Core.Models
{
    public class RoleModel
    {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public List<UserRoleModel> UserRoles { get; set; } = new List<UserRoleModel>();
    }
}
