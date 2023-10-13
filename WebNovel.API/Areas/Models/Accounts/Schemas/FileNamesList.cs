using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Areas.Models.Accounts.Schemas
{
    public class FileNamesList
    {
        public string Folder {get; set;}
        public List<string> FileName {get; set;}
    }
}