using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Commons.Enums
{
    public class MSG_NO
    {
        /// <summary>
        /// Tên người dùng này đã được đăng ký. Vui lòng sử dụng tên người dùng khác!
        /// </summary>
        public static readonly string USERNAME_HAD_USED = "E015";

        /// <summary>
        /// Xác nhận mật khẩu không khớp, vui lòng kiểm tra lại.
        /// </summary>
        public static readonly string CONFIRM_PASSWORD_INVALIDATE = "E018";
        /// <summary>
        /// Dữ liệu được yêu cầu không được tìm thấy.
        /// </summary>
        public static readonly string NOT_FOUND = "E404";
    }
}