using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Areas.Models.Reports.Schemas
{
    public class ReportDto
    {
        public long Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string AccountReport { get; set; } = string.Empty;
        public string NovelId { get; set; } = string.Empty;
        public string NovelTitle { get; set; } = string.Empty;
        public string AccountIdOfNovelId { get; set; } = string.Empty;
        public string AccountOfNovel { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}