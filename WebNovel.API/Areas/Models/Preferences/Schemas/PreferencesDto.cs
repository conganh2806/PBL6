namespace WebNovel.API.Areas.Models.Preferences.Schemas
{
    public class PreferencesDto
    {
        public long Id { get; set; }
        public long NovelId { get; set; }
        public long AccountId { get; set; }
    }
}