using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Core.Services.Schemas
{
    public class EmailRequest
    {
        public string ToMail { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string Template { get; set; } = default!;
        public string JsonData { get; set; } = default!;
    }
}