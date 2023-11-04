using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebNovel.API.Databases
{
        /// <summary>
        /// Bảng có khóa chính là kiểu int và tự động tăng.
        /// <para>Author: Quynh</para>
        /// <para>Created: 9/10/2023</para>
        /// </summary>
        public class TableHaveIdString : Table
        {
                /// <summary>
                /// Id định danh (khóa chính)
                /// </summary>
                [Key]
                [Comment("Id định danh (khóa chính)")]
                [StringLength(21)]
                public string Id { set; get; }
        }
}