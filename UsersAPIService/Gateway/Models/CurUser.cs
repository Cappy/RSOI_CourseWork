using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gateway.Models
{
    public class CurUser
    {
        public string access_token { get; set; }
        public string userid { get; set; }
        public string email { get; set; }
        public List<string> roles { get; set; }
    }
}
