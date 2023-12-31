

using System.ComponentModel.DataAnnotations;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Areas.Models.Novels.Schemas
{
        public class NovelCreateEntity
        {
                [StringLength(255, ErrorMessage = "E005")]
                public string Name { get; set; } = null!;
                public string Title { get; set; } = null!;
                public string AccountId { get; set; } = null!;
                [StringLength(10000, ErrorMessage = "E005, Description is too long")]
                public string? Description { get; set; }
                public List<long> GenreIds { get; set; } = null!;
                public IFormFile File { get; set; } = null!;
                public IFormFile? BackgroundFile { get; set; } = null!;
        }
        public class NovelUpdateEntity
        {
                public string Id { get; set; } = null!;
                [StringLength(255, ErrorMessage = "E005")]
                public string? Name { get; set; } = null!;
                [StringLength(255, ErrorMessage = "E005")]
                public string? Title { get; set; } = null!;
                [StringLength(10000, ErrorMessage = "E005, Description is too long")]
                public string? Description { get; set; }
                public bool? Status { get; set; }
                public bool? ApprovalStatus { get; set; }

                public List<long>? GenreIds { get; set; } = null!;
                public IFormFile? File { get; set; } = null!;
                public IFormFile? BackgroundFile { get; set; } = null!;
        }
        public class NovelDeleteEntity
        {
                public string Id { get; set; } = null!;
        }
        public class NovelCreateUpdateEntity
        {
                public string? Id { get; set; }
                [StringLength(255, ErrorMessage = "E005")]
                public string Name { get; set; } = null!;
                [StringLength(255, ErrorMessage = "E005")]
                public string? Title { get; set; }
                public string AccountId { get; set; }
                public int Year { get; set; }
                public int Views { get; set; }
                public int Rating { get; set; }
                [MaxLength()]
                public string? ImagesURL { get; set; }
                [StringLength(10000, ErrorMessage = "E005, Description is too long")]
                public string? Description { get; set; }
                public bool Status { get; set; }
                public bool ApprovalStatus { get; set; }
                public List<long>? GenreIds { get; set; }
                public IFormFile? File { get; set; }
        }
}