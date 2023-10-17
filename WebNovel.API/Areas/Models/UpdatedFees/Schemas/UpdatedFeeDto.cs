namespace WebNovel.API.Areas.Models.UpdatedFees.Schemas
{
    public class UpdatedFeeDto
    {
        public long Id { get; set; }
        public float Fee { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}