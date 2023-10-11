using System.ComponentModel.DataAnnotations;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Databases.Entitites
{
    public class Novel : TableHaveIdInt
    {
        public Novel() {
            this.Genres = new HashSet<Genre>();
            this.Comments = new HashSet<Comment>();
            this.Chapters = new HashSet<Chapter>();
            this.Accounts = new HashSet<Account>();
            this.Preferences = new HashSet<Preferences>();
        }


        [StringLength(255)] 
        public string Name {get; set;} = null!;
        [StringLength(255)]
        public string Title {get; set;} = null!;
        public long AccountId {get; set;}
        public Account? Account {get; set;}
        public int? Year {get; set;}
        public int Views {get; set;}
        public int Rating {get; set;} 
        [StringLength(500)]
        public string Description {get; set;} = null!;
        public bool? Status {get; set;} //Trạng thái của truyện dùng để cài đặt trạng thái còn trong giai đoạn sáng tác hay k
        public bool ApprovalStatus {get; set;}
        public virtual ICollection<Genre> Genres {get; set;} = null!;
        public virtual ICollection<Comment>? Comments {get; set;}
        public virtual ICollection<Chapter>? Chapters {get; set;} 
        public virtual ICollection<Account>? Accounts {get; set;}
        public virtual ICollection<Preferences>? Preferences {get; set;}     
    }



}