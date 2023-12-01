namespace WebNovel.API.Areas.Models.Comment.Schemas
{
    public class CommentDto
    {
        public long Id { get; set; }
        public string AccountId { get; set; } = null!;
        public string NovelId { get; set; } = null!;
        public string Text { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? NickName { get; set; } = null!;
        public List<string>? RoleIds { set; get; } = null!;
        public DateTime? CreateOn { get; set; }
    }
}