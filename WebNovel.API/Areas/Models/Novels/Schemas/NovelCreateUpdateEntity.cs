

using System.ComponentModel.DataAnnotations;

namespace WebNovel.API.Areas.Models.Novels.Schemas
{
    public class NovelCreateUpdateEntity
    {
        public long Id {get; set;}
        [StringLength(255, ErrorMessage = "E005")]
        public string Name {get; set;} = null!;
        [StringLength(255, ErrorMessage = "E005")]
        public string Title {get; set;} = null!;
        public long AccountId {get; set;}
        public int Year {get; set;}
        public int Views {get; set;}
        public int Rating {get; set;}
        [StringLength(255, ErrorMessage = "E005")]
        public string? Description {get; set;}
        public bool Status {get; set;}
        public bool ApprovalStatus {get; set;}
    }
}