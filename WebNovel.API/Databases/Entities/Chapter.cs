using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class Chapter : TableHaveIdString
    {
        public Chapter()
        {
            Bookmarked = new HashSet<Bookmarked>();
        }
        [StringLength(255)]
        public string Name { get; set; } = null!;
        public bool IsLocked { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime PublishDate { get; set; }
        public int Views { get; set; }
        public int Rating { get; set; }
        public long FeeId { get; set; }
        public virtual UpdatedFee UpdatedFee { get; set; } = null!;
        public string? FileContent { get; set; }
        public int? Discount { get; set; }
        public bool ApprovalStatus { get; set; }
        public string NovelId { get; set; }
        public virtual Novel Novel { get; set; } = null!;
        public virtual ICollection<Bookmarked>? Bookmarked { get; set; }

    }
}