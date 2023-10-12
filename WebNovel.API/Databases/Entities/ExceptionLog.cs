using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Databases.Entities
{
    [Table("ExceptionLogs")]
    public class ExceptionLog : TableHaveIdInt
    {
        [StringLength(255)]
        public string Project { set; get; }

        [StringLength(255)]
        public string Class { set; get; }

        [StringLength(255)]
        public string Method { set; get; }

        public string Message { set; get; }

        public string Data { set; get; }

        public string Source { set; get; }

        public string StackTrace { set; get; }

        public string InnerException { set; get; }
    }
}