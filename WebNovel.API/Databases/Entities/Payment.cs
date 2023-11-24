using System;

namespace WebNovel.API.Databases.Entities
{
    public class Payment : TableHaveIdString
    {
        public Payment()
        {
            PaymentNotifications = new HashSet<PaymentNotification>();
            PaymentSignatures = new HashSet<PaymentSignature>();
            PaymentTransactions = new HashSet<PaymentTransaction>();
        }
        public string PaymentContent { get; set; } = string.Empty;
        public string PaymentCurrency { get; set; } = string.Empty;
        public string PaymentRefId { get; set; } = string.Empty;
        public decimal? RequiredAmount { get; set; }
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        public DateTime? ExpireDate { get; set; }
        public string? PaymentLanguage { get; set; } = string.Empty;
        public string? MerchantId { get; set; } = string.Empty;
        public string? PaymentDestinationId { get; set; } = string.Empty;
        public decimal? PaidAmount { get; set; }
        public string? PaymentStatus { get; set; } = string.Empty;
        public string? PaymentLastMessage { get; set; } = string.Empty;

        public virtual Merchant Merchant { get; set; } = null!;
        public virtual ICollection<PaymentNotification> PaymentNotifications { get; set; } = null!;
        public virtual PaymentDestination PaymentDestination { get; set; } = null!;
        public virtual ICollection<PaymentSignature> PaymentSignatures { get; set; } = null!;
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = null!;
    }
}
