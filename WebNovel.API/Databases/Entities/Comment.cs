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
        public class Comment : Table
        {
                public string AccountId { get; set; }
                public virtual Account Account { get; set; } = null!;
                public string NovelId { get; set; }
                public virtual Novel Novel { get; set; } = null!;
                [Required]
                public string Text { get; set; } = null!;
                [DataType(DataType.DateTime)]
                public DateTime? CreateOn { get; set; }

        }
}