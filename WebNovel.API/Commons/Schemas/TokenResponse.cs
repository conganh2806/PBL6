using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Commons.Schemas
{
    public class TokenResponse
    {
        public string? Token {get; set;}
        public string? RefreshToken {get; set;}
        public DateTime RefreshTokenExpiryTime {get; set;}
    }
}