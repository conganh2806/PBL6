namespace WebNovel.API.Areas.Models.Preferences.Schemas
{
    public class PreferencesCreateUpdateEntity
    {
        public long NovelId { get; set; }
        public long AccountId { get; set; }
    }
}