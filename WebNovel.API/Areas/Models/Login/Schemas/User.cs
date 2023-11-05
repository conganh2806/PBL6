using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebNovel.API.Areas.Models.Login.Schemas
{
    public class UserEntity
    {
        /// <summary>
        /// Tên đăng nhập, email hoặc số điện thoại
        /// </summary>
        /// <example>root</example>
        [Required(ErrorMessage = "E001")]
        [StringLength(50, ErrorMessage = "E005")]
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Mật khẩu đăng nhập đã mã hoá MD5
        /// </summary>
        /// <example>628c709b5d084dc6b22a6dbe87665419</example>
        [Required(ErrorMessage = "E001")]
        [StringLength(50, ErrorMessage = "E005")]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}