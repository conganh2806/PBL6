using System;
using System.ComponentModel.DataAnnotations;

namespace WebNovel.API.Areas.Models.Orders.Schemas
{
    public class OrderCreateEntity
    {
        public string AccountId { get; set; } = null!;
        public long BundleId { get; set; }
    }
    public class OrderUpdateEntity
    {
        public string Id { set; get; } = null!;
        public string? AccountId { get; set; } = null!;
        public long? BundleId { get; set; }
    }
    public class OrderDeleteEntity
    {
        public string Id { set; get; } = null!;
    }
    public class OrderCreateUpdateEntity
    {

    }
}
