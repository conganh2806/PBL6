using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Commons.CodeMaster
{
    /// <summary>
    /// A001 Account Status Class.<br/>
    /// !!! DO NOT MODIFY !!!
    /// <list type="bullet">
    /// <item>
    /// <term>NORMAL</term>
    /// <description>0 : Đang sử dụng</description>
    /// </item>
    /// <item>
    /// <term>TEMP_LOCK</term>
    /// <description>10 : Tạm khoá</description>
    /// </item>
    /// <item>
    /// <term>LOCK</term>
    /// <description>20 : Khoá</description>
    /// </item>
    /// <item>
    /// <term>BLACKLIST</term>
    /// <description>30 : Danh sách đen</description>
    /// </item>
    /// <item>
    /// <term>CANCEL</term>
    /// <description>40 : Huỷ tài khoản</description>
    /// </item>
    /// </list>
    /// </summary>
    public static class A001
    {

        /// <summary>
        /// 0 : Đang sử dụng
        /// </summary>
        public static class NORMAL
        {
            public static readonly int CODE = 0;
            public static readonly string NAME = "Using";
        }

        /// <summary>
        /// 10 : Tạm khoá
        /// </summary>
        public static class TEMP_LOCK
        {
            public static readonly int CODE = 10;
            public static readonly string NAME = "Temp Locking";
        }

        /// <summary>
        /// 20 : Khoá
        /// </summary>
        public static class LOCK
        {
            public static readonly int CODE = 20;
            public static readonly string NAME = "Locking";
        }

        /// <summary>
        /// 30 : Danh sách đen
        /// </summary>
        public static class BLACKLIST
        {
            public static readonly int CODE = 30;
            public static readonly string NAME = "Blacklist";
        }

        /// <summary>
        /// 40 : Huỷ tài khoản
        /// </summary>
        public static class CANCEL
        {
            public static readonly int CODE = 40;
            public static readonly string NAME = "Cancel";
        }
    }
}