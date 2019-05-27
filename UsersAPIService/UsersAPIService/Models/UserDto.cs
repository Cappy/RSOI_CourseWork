using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersAPIService.Models
{
    public class UserDto
    {
        public Guid Userid { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsRentlord { get; set; }
    }
}

