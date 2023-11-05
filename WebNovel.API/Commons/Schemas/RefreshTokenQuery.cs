using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Commons.Schemas
{
    public class RefreshTokenQuery
    {
        public string? Token {get; set;}
        public string? RefreshToken {get; set;}
    }
}