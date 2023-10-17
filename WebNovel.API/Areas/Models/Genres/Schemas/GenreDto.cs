using System.ComponentModel.DataAnnotations;

namespace WebNovel.API.Areas.Models.Genres.Schemas
{
    public class GenreDto
    {
        public long Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = null!;
    }
}