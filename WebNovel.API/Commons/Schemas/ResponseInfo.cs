using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Commons.Schemas
{
    public class ResponseInfo
    {
        /// <summary>
        /// Mã trả về tương ứng cho xử lý
        /// <para>200: Thành công</para>
        /// <para>201: Lỗi dữ liệu nhập vào</para>
        /// <para>202: Có lỗi khác phát sinh</para>
        /// <para>403: Không có quyền truy cập</para>
        /// <para>500: Lỗi server</para>
        /// </summary>
        public int Code { set; get; }
        /// <summary>
        /// Mã của thông báo sẽ hiển thị
        /// </summary>
        public string MsgNo { set; get; }

        public ResponseInfo()
        {
            Code = CodeResponse.OK;
            MsgNo = "";
            Data = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Data { set; get; }
    }
}