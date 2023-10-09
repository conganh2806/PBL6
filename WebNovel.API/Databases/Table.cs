using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebNovel.API.Databases
{
    /// <summary>
    /// Dữ liệu chung cho các table trong hệ thống (thông tin việc tạo, update và delete dữ liệu).
    /// <para>Author: Quynh</para>
    /// <para>Created: 9/10/2023</para>
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Ngày tạo dữ liệu
        /// </summary>
        [Comment("Ngày tạo dữ liệu")]
        public DateTimeOffset CreatedAt { set; get; }

        /// <summary>
        /// Ngày cập nhật dữ liệu
        /// </summary>
        [Comment("Ngày cập nhật dữ liệu")]
        public DateTimeOffset? UpdatedAt { set; get; }

        /// <summary>
        /// Ngày xoá dữ liệu
        /// </summary>
        [Comment("Ngày xoá dữ liệu")]
        public DateTimeOffset? DeletedAt { set; get; }

        /// <summary>
        /// Cờ xóa dữ liệu
        /// </summary>
        [Comment("Cờ xóa dữ liệu")]
        public bool DelFlag { set; get; }
        /// <summary>
        /// Xoá vật lý dữ liệu
        /// </summary>
        [NotMapped]
        public bool ForceDel { set; get; } = false;
    }
}