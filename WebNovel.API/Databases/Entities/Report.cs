using System;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class Report : TableHaveIdInt
    {
        public string AccountId { get; set; } = string.Empty;
        public virtual Account Account { get; set; } = null!;
        public string NovelId { get; set; } = string.Empty;
        public virtual Novel Novel { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
