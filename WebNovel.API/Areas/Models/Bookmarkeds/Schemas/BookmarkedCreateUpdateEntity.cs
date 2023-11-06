namespace WebNovel.API.Areas.Models.Bookmarked.Schemas
{
    public class BookmarkedCreateUpdateEntity
    {
        public string AccountId { get; set; } = null!;
        public string NovelId { get; set; } = null!;
        public string ChapterId { get; set; } = null!;
    }
}