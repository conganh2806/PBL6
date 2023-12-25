using System;

namespace WebNovel.API.Databases.Entities
{
    public class Merchant : TableHaveIdString
    {
        public Merchant()
        {
            PaymentNotifications = new HashSet<PaymentNotification>();
            Payments = new HashSet<Payment>();
        }
        public string? MerchantName { get; set; } = string.Empty;
        public string? MerchantWebLink { get; set; } = string.Empty;
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        public string? MerchantReturnUrl { get; set; } = string.Empty;
        public string? SecretKey { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public virtual ICollection<PaymentNotification> PaymentNotifications { get; set; } = null!;
        public virtual ICollection<Payment> Payments { get; set; } = null!;
    }
}
