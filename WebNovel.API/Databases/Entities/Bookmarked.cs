using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class Bookmarked : Table
    {
        public Bookmarked()
        {
            ForceDel = true;
        }
        public string AccountId { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
        public string NovelId { get; set; } = null!;
        public virtual Novel Novel { get; set; } = null!;
        public string ChapterId { get; set; } = null!;
        public virtual Chapter Chapter { get; set; } = null!;
    }
}