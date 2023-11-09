using WebNovel.API.Areas.Models.Novels.Schemas;

namespace WebNovel.API.Areas.Models.Preferences.Schemas
{
    public class PreferencesDto
    {
        public string NovelId { get; set; }
        public string AccountId { get; set; }
        public NovelDto Novel { get; set; } = null!;
    }
}