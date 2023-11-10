using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Areas.Models.Login.Schemas
{
    public class GoogleOauthUser
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Picture { get; set; }
        public bool EmailVeriFied { get; set; }
    }
}