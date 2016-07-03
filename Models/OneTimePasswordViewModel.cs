using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassPerfectWebAPI.Models
{
    public class OneTimePasswordViewModel
    {
        public string OneTimePassCipherText { get; set; }
        public string GeneratedPassword { get; set; }
    }
}