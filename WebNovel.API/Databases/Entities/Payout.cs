using System;
using System.ComponentModel.DataAnnotations;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class Payout : TableHaveIdInt
    {
        public Payout()
        {
            ForceDel = true;
        }
        [StringLength(36)]
        public string AccountId { set; get; } = string.Empty;
        public float PayoutAmount { get; set; }
        public bool PayoutStatus { get; set; }
        public string Bank { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankNumber { get; set; } = string.Empty;

        public virtual Account Account { get; set; } = null!;
    }
}
