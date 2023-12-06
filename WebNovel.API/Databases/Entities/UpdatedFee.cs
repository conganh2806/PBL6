using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebNovel.API.Databases.Entities
{
    public class UpdatedFee : TableHaveIdInt
    {
        public UpdatedFee()
        {
            this.Chapters = new HashSet<Chapter>();
        }
        public float Fee { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateUpdated { get; set; }
        public int Year { get; set; }
        public virtual ICollection<Chapter>? Chapters { get; set; }

    }
}