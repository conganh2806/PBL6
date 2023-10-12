using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Commons.CodeMaster
{
    /// <summary>
    /// R001 Default Role Class.<br/>
    /// !!! DO NOT MODIFY !!!
    /// <list type="bullet">
    /// <item>
    /// <term>DEVELOPER</term>
    /// <description>DEV : Role Developer</description>
    /// </item>
    /// <item>
    /// <term>ADMIN</term>
    /// <description>ADMIN : Role Admin</description>
    /// </item>
    /// <item>
    /// <term>CREATOR</term>
    /// <description> : Role CREATOR</description>
    /// </item>
    /// <item>
    /// <term>READER</term>
    /// <description>EMP : Role READER</description>
    /// </item>
    /// </list>
    /// </summary>
    public static class R001
    {

        /// <summary>
        /// DEV : Role Developer
        /// </summary>
        public static class DEVELOPER
        {
            public static readonly string CODE = "DEV";
            public static readonly string NAME = "Lập trình viên";
        }

        /// <summary>
        /// ADMIN : Role Admin
        /// </summary>
        public static class ADMIN
        {
            public static readonly string CODE = "ADMIN";
            public static readonly string NAME = "Quản lý";
        }

        /// <summary>
        /// CREATOR : Role CREATOR
        /// </summary>
        public static class CREATOR
        {
            public static readonly string CODE = "CREATOR";
            public static readonly string NAME = "Roll đăng truyện";
        }

        /// <summary>
        /// READER : Role READER
        /// </summary>
        public static class READER
        {
            public static readonly string CODE = "READER";
            public static readonly string NAME = "Roll đọc truyện";
        }
    }
}