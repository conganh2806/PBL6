using System;

namespace WebNovel.API.Areas.Models.Payments.Schemas
{
    public class PaymentDto
    {
        public string Id { get; set; } = string.Empty;
        public string PaymentContent { get; set; } = string.Empty;
        public string PaymentCurrency { get; set; } = string.Empty;
        public string PaymentRefId { get; set; } = string.Empty;
        public decimal? RequiredAmount { get; set; }
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        public DateTime? ExpireDate { get; set; }
        public string? PaymentLanguage { get; set; } = string.Empty;
        public string? MerchantId { get; set; } = string.Empty;
        public string? PaymentDestinationId { get; set; } = string.Empty;
        public string? PaymentStatus { get; set; } = string.Empty;
        public decimal? PaidAmount { get; set; }
    }

    public class PaymentHistoryDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;

        public string AccountId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public long BundleId { get; set; }
        public float CoinAmount { get; set; }
        public decimal Price { get; set; }

        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        public string? PaymentStatus { get; set; } = string.Empty;
    }

    public class PurchaseHistoryDto
    {
        public string AccountId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string ChapterId { get; set; } = null!;
        public string ChapterName { get; set; } = null!;

        public string NovelId { get; set; } = null!;
        public string NovelTitle { get; set; } = null!;

        public long? FeeId { get; set; } = null!;
        public float Fee { get; set; }

        public DateTimeOffset purchaseDate { get; set; }
    }
}
