using System;

namespace WebNovel.API.Areas.Models.Orders.Schemas
{
    public class OrderDto
    {
        public string Id { set; get; } = null!;
        public string AccountId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public long BundleId { get; set; }
        public float CoinAmount { get; set; }
        public decimal Price { get; set; }
    }
}
