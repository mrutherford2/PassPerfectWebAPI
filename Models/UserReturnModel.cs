using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace PassPerfectWebAPI.Models
{
    public class UserReturnModel
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime JoinDate { get; set; }
        public IList<string> Roles { get; set;}
        public IList<Claim> Claims { get; set; }
        public string PublicKeyPath { get; set; }
        public string OTPHash { get; set; }
    }
}