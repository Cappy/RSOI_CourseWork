using System;
using System.Collections.Generic;

namespace Gateway.Models
{
    public partial class Users
    {
        public Guid Userid { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsRentlord { get; set; }
    }
}
