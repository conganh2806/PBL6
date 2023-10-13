using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Core.Schemas
{
    public class FileInput
    {
        public IFormFile? file { set; get; }

        public string folder { set; get; }

        public string fileName { set; get; }
    }
}