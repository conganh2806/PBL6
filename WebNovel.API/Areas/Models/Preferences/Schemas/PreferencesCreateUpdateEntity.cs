namespace WebNovel.API.Areas.Models.Preferences.Schemas
{
    public class PreferencesCreateUpdateEntity
    {
        public string NovelId { get; set; } = null!;
        public string AccountId { get; set; } = null!;
    }

    public class PreferencesDeleteEntity
    {
        public string NovelId { get; set; } = null!;
        public string AccountId { get; set; } = null!;
    }
}