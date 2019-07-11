using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Infrastructure.Entities
{
    public class SocialLogin
    {
        public int SocialLoginId { get; set; }
        public string Provider { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
