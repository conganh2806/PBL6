using System;

namespace WebNovel.API.Databases.Entities
{
    public class PaymentSignature : TableHaveIdString
    {
        public string? PaymentId { get; set; } = string.Empty;
        public string? SignValue { get; set; } = string.Empty;
        public string? SignAlgo { get; set; } = string.Empty;
        public string? SignOwn { get; set; } = string.Empty;
        public DateTime? SignDate { get; set; }
        public bool IsValid { get; set; }

        public virtual Payment Payment { get; set; } = null!;
    }
}
