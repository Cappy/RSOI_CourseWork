using System;
using System.Collections.Generic;

namespace UsersAPIService.Models
{
    public partial class OAuth2Tokens
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime IssuedAt { get; set; }
        public int Revoked { get; set; }
    }
}

