using System;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class ChapterOfAccount : Table
    {
        public string AccountId { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
        public string ChapterId { get; set; } = null!;
        public virtual Chapter Chapter { get; set; } = null!;
        public string NovelId { get; set; } = null!;
        public virtual Novel Novel { get; set; } = null!;
    }
}
