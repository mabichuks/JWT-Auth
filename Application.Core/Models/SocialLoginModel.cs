using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Core.Models
{
    public class SocialLoginModel
    {
        public int SocialLoginId { get; set; }
        public string Provider { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public UserModel User { get; set; }
    }
}
