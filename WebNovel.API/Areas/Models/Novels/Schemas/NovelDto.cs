using System.ComponentModel.DataAnnotations;

namespace WebNovel.API.Areas.Models.Novels.Schemas
{
    public class NovelDto
    {
        public long Id {get; set;}
        [StringLength(255)]
        public string Name {get; set;} = null!;
        [StringLength(255)]
        public string Title {get; set;} = null!;
        public string Author {get; set;} = null!;
        public int Year {get; set;}
        public int Views {get; set;}
        public int Rating {get; set;}
        [MaxLength()] 
        public string ImagesURL {get; set;} = null!;
        public List<string> GenreName {get; set;} = null!;
        [StringLength(500)]
        public string? Description {get; set;}
        public bool Status {get; set;}
        public bool ApprovalStatus {get; set;}
        public List<long> GenreIds {get; set;}

    }
}