namespace WebNovel.API.Areas.Models.Rating.Schemas
{
    public class RatingDto
    {
        public long NovelId { get; set; }
        public long AccountId { get; set; }
        public float RateScore { get; set; }
    }
}