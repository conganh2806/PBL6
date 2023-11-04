using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Databases.Entitites
{
        public class Rating : Table
        {
                public string NovelId { get; set; }
                public string AccountId { get; set; }
                public float RateScore { get; set; }
                public virtual Novel Novel { get; set; } = null!;
                public virtual Account Account { get; set; } = null!;
        }
}