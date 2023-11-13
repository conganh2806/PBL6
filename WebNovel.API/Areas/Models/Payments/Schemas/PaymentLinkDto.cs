using System;

namespace WebNovel.API.Areas.Models.Payments.Schemas
{
    public class PaymentLinkDto
    {
        public string PaymentId { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
