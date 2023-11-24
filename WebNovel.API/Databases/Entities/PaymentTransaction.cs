using System;

namespace WebNovel.API.Databases.Entities
{
    public class PaymentTransaction : TableHaveIdString
    {
        public string? TranMessage { get; set; } = string.Empty;
        public string? TranPayload { get; set; } = string.Empty;
        public string? TranStatus { get; set; } = string.Empty;
        public decimal? TranAmount { get; set; }
        public DateTime? TranDate { get; set; }
        public string? PaymentId { get; set; } = string.Empty;
        public string? TranRefId { get; set; } = string.Empty;
        public virtual Payment Payment { get; set; } = null!;
    }
}
