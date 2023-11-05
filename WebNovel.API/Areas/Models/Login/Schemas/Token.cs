using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebNovel.API.Areas.Models.Login.Schemas
{
    public class Token
    {
        /// <summary>
        /// Token ngẫu nhiên của phiên đăng nhập hiện tại
        /// </summary>
        [Required(ErrorMessage = "E001")]
        [StringLength(500, ErrorMessage = "E005")]
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
