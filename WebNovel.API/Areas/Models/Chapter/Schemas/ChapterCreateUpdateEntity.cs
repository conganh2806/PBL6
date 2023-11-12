using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Areas.Models.Chapter.Schemas
{
    public class ChapterCreateEntity
    {
        public string Name { get; set; } = null!;
        public string NovelId { get; set; } = null!;
        public IFormFile File { get; set; } = null!;
    }

    public class ChapterUpdateEntity
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public IFormFile? File { get; set; } = null!;
        public bool? IsLocked { get; set; }
        public int? Views { get; set; }
        public long? FeeId { get; set; }
        public int? Discount { get; set; }
        public bool? ApprovalStatus { get; set; }
        public bool? IsPublished { get; set; }
    }

    public class ChapterCreateUpdateEntity
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsLocked { get; set; }
        public DateTime? PublishDate { get; set; }
        public int Views { get; set; }
        public int Rating { get; set; }
        public long FeeId { get; set; }
        [MaxLength]
        public string? FileContent { get; set; }
        public int? Discount { get; set; }
        public bool ApprovalStatus { get; set; }
        public string NovelId { get; set; }
        public IFormFile File { get; set; }
    }
}