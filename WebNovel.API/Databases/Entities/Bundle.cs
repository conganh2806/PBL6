using System;

namespace WebNovel.API.Databases.Entities
{
    public class Bundle : TableHaveIdInt
    {
        public Bundle()
        {
            Orders = new HashSet<Order>();
        }
        public float CoinAmount { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
