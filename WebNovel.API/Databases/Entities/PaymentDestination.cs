using System;

namespace WebNovel.API.Databases.Entities
{
    public class PaymentDestination : TableHaveIdString
    {
        public PaymentDestination()
        {
            Payments = new HashSet<Payment>();
        }
        public string? DesName { get; set; } = string.Empty;
        public string? DesShortName { get; set; } = string.Empty;
        public string? DesParentId { get; set; } = string.Empty;
        public string? DesLogo { get; set; } = string.Empty;
        public int SortIndex { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = null!;
    }
}
