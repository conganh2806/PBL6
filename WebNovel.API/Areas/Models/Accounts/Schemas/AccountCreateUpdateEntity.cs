using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Areas.Models.Accounts.Schemas
{
    public class AccountCreateUpdateEntity
    {
        public long Id {get; set;}
        [StringLength(100, ErrorMessage = "E005")]
        public string Username {get; set;} = null!;
        [StringLength(255, ErrorMessage = "E005")]
        public string Password {get; set;} = null!;
        [StringLength(255, ErrorMessage = "E005")]
        public string? Email {get; set;}
        [StringLength(100, ErrorMessage = "E005")]
        public string NickName {get; set;} = null!;
        public string ConfirmPassword {get; set;} = null!;
        public string? Phone {get; set;}
        public float WalletAmmount {get; set;} = 0.0f;
        public List<string> RoleIds { set; get; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}