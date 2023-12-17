using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Areas.Models.Payments.Schemas
{
    public class PayoutDto
    {
        public string AccountId { set; get; } = string.Empty;
        public float PayoutAmount { get; set; }
        public bool PayoutStatus { get; set; }
        public string Bank { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankNumber { get; set; } = string.Empty;
    }
}