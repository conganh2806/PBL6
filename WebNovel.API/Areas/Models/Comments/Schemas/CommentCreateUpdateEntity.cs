namespace WebNovel.API.Areas.Models.Comment.Schemas
{
    public class CommentCreateUpdateEntity
    {
        public string AccountId { get; set; } = null!;
        public string NovelId { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime? CreateOn { get; set; }
    }
}