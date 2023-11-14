namespace WebNovel.API.Areas.Models.Comment.Schemas
{

    public class CommentCreateEntity
    {
        public string AccountId { get; set; } = null!;
        public string NovelId { get; set; } = null!;
        public string Text { get; set; } = null!;
    }
    public class CommentUpdateEntity
    {
        public long Id { get; set; }
        public string Text { get; set; } = null!;
    }

    public class CommentDeleteEntity
    {
        public long Id { get; set; }
    }

    public class CommentCreateUpdateEntity
    {
        public long? Id { get; set; }
        public string AccountId { get; set; } = null!;
        public string NovelId { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime? CreateOn { get; set; }
    }
}