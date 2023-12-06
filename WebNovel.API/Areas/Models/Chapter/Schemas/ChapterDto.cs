using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Areas.Models.Chapter.Schemas
{
    public class ChapterDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsLocked { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
        public int Views { get; set; }
        public long? FeeId { get; set; }
        public float? Fee { get; set; }
        [MaxLength]
        public string? FileContent { get; set; }
        public int? Discount { get; set; }
        public bool ApprovalStatus { get; set; }
        public string NovelId { get; set; }
        public int ChapIndex { get; set; }
    }
}