using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Areas.Models.Accounts.Schemas
{
    public class AccountDto
    {
        public string Id { get; set; } = null!;
        [StringLength(100)]
        public string Username { get; set; } = null!;
        [StringLength(255)]
        public string Email { get; set; } = null!;
        public string NickName { get; set; } = null!;
        public int Status { get; set; }
        public string? Phone { get; set; }
        public List<string> RoleIds { set; get; } = null!;
        public bool IsAdmin { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}