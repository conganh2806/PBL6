namespace WebNovel.API.Areas.Models.Rating.Schemas
{
        public class RatingCreateUpdateEntity
        {
                public string NovelId { get; set; } = null!;
                public string AccountId { get; set; } = null!;
                public float RateScore { get; set; }
        }

        public class RatingDeleteEntity
        {
                public string NovelId { get; set; } = null!;
                public string AccountId { get; set; } = null!;
        }
}