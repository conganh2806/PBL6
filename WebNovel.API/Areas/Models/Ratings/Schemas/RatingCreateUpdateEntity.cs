namespace WebNovel.API.Areas.Models.Rating.Schemas
{
    public class RatingCreateUpdateEntity
    {
        public long NovelId { get; set; }
        public string AccountId { get; set; }
        public float RateScore { get; set; }
    }
}