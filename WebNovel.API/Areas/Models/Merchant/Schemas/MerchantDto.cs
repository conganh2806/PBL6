using System;

namespace WebNovel.API.Areas.Models.Merchant.Schemas
{
    public class MerchantDto
    {
        public string? Id { get; set; } = string.Empty;
        public string? MerchantName { get; set; } = string.Empty;
        public string? MerchantWebLink { get; set; } = string.Empty;
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        public string? MerchantReturnUrl { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
