using System.ComponentModel.DataAnnotations;

namespace WebNovel.API.Areas.Models.Genres.Schemas
{
    public class GenreCreateUpdateEntity
    {
        public long Id { get; set; }
        [StringLength(100, ErrorMessage = "E005")]
        public string Name { get; set; } = null!;
    }

    public class GenreDeleteEntity
    {
        public long Id { get; set; }
    }
}