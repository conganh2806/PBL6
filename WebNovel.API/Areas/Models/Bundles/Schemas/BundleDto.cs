using System;

namespace WebNovel.API.Areas.Models.Bundles.Schemas
{
    public class BundleDto
    {
        public long Id { set; get; }
        public float CoinAmount { get; set; }
        public decimal Price { get; set; }
    }
}
