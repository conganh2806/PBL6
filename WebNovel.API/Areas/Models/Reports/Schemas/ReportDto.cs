using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Areas.Models.Reports.Schemas
{
    public class ReportDto
    {
        public long Id  {get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string NovelId { get; set; } = string.Empty;
        public string NameNovel { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}