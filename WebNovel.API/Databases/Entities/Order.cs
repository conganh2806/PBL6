using System;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class Order : TableHaveIdString
    {
        public string AccountId { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
        public long BundleId { get; set; }
        public virtual Bundle Bundle { get; set; } = null!;
    }
}
