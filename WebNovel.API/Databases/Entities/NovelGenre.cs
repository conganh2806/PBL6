using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class NovelGenre : Table
    {
        public NovelGenre()
        {
            ForceDel = true;
        }
        [Key]
        [Column(Order = 1)]
        public string NovelId { get; set; }

        [Key]
        [Column(Order = 2)]
        public long GenreId { get; set; }
        public virtual Novel Novel { set; get; }

        public virtual Genre Genre { set; get; }
    }
}