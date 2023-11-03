using System.ComponentModel.DataAnnotations;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Databases.Entitites
{
    public class Novel : TableHaveIdString
    {
        public Novel()
        {
            this.Genres = new HashSet<NovelGenre>();
            this.Comments = new HashSet<Comment>();
            this.Chapters = new HashSet<Chapter>();
            this.Preferences = new HashSet<Preferences>();
            this.Ratings = new HashSet<Rating>();
        }


        [StringLength(255)]
        public string Name { get; set; } = null!;
        [StringLength(255)]
        public string Title { get; set; } = null!;
        public long AccountId { get; set; }
        public Account? Account { get; set; }
        public int Year { get; set; }
        public int Views { get; set; }
        public int Rating { get; set; }
        [StringLength(500)]
        public string? ImageURL { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public bool Status { get; set; } //Trạng thái của truyện dùng để cài đặt trạng thái còn trong giai đoạn sáng tác hay k
        public bool ApprovalStatus { get; set; }
        public virtual ICollection<NovelGenre> Genres { get; set; } = null!;
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Chapter>? Chapters { get; set; }
        public virtual ICollection<Preferences>? Preferences { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }



}