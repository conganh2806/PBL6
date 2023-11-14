using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Databases.Entities
{
    public class Batch
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string Id { set; get; }

        [Required]
        [MaxLength(100)]
        public string Name { set; get; }

        public int Timeout { get; set; }

        [Required]
        [MaxLength(50)]
        public string CronExpression { get; set; }

        public DateTimeOffset? LastExecution { get; set; }

        public DateTimeOffset? NextExecution { get; set; }

        public int Status { get; set; }
    }
}