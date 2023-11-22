using System;

namespace WebNovel.API.Areas.Models.Bundles.Schemas
{
    public class BundleCreateEntity
    {
        public float CoinAmount { get; set; }
        public decimal Price { get; set; }
    }

    public class BundleDeleteEntity
    {
        public long Id { set; get; }
    }
    public class BundleUpdateEntity
    {
        public long Id { set; get; }
        public float? CoinAmount { get; set; }
        public decimal? Price { get; set; }
    }

    public class BundleCreateUpdateEntity
    {

    }
}
