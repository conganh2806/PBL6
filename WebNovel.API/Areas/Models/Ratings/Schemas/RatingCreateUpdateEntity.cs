namespace WebNovel.API.Areas.Models.Rating.Schemas
{
    public class RatingCreateUpdateEntity
    {
        public string NovelId { get; set; }
        public long AccountId { get; set; }
        public float RateScore { get; set; }
    }
}