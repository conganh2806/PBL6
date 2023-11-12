using WebNovel.API.Areas.Models.Novels.Schemas;

namespace WebNovel.API.Areas.Models.Preferences.Schemas
{
    public class PreferencesDto
    {
        public string NovelId { get; set; } = null!;
        public string AccountId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public int Year { get; set; }
        public int Views { get; set; }
        public int Rating { get; set; }
        public string ImagesURL { get; set; } = null!;
        public List<string> GenreName { get; set; } = null!;
        public List<long> GenreIds { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public bool ApprovalStatus { get; set; }
        public int NumChapter { get; set; }
    }
}