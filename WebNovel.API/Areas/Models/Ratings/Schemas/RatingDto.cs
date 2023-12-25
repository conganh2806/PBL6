namespace WebNovel.API.Areas.Models.Rating.Schemas
{
        public class RatingDto
        {
                public string NovelId { get; set; } = null!;

                public string AccountId { get; set; } = null!;

                public float RateScore { get; set; }
                public string Username { get; set; } = null!;
                public string Email { get; set; } = null!;
                public string? NickName { get; set; } = null!;
                public List<string>? RoleIds { set; get; } = null!;
        }
}