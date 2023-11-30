using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Core.Services.Schemas
{
    public class MailSettings
    {
        public string Host { get; set; } = default!;

        public bool Ssl { get; set; }

        public int Port { get; set; }

        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string Name { get; set; } = default!;
    }
}