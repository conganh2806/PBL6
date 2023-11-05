namespace WebNovel.API.Areas.Models.Rating.Schemas
{
        public class RatingDto
        {
                public string NovelId { get; set; }
                public string AccountId { get; set; }
                public float RateScore { get; set; }
        }
}