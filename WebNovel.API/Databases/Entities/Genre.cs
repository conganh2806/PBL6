using System.ComponentModel.DataAnnotations;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class Genre : TableHaveIdInt
    {
        public Genre() {
            this.Novels = new HashSet<Novel>();
        }

        [StringLength(100)]
        public string Name {get; set;} = null!;
        public virtual ICollection<Novel> Novels {get; set;} = null!;
    }
}