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
        public long Id {get; set;}
        [StringLength(100)]
        public string Username {get; set;}
        [StringLength(255)]
        public string Password {get; set;} = null!;
        [StringLength(255)]
        public string Email {get; set;} = null!;
        public string RoleName {get; set;} = null!;
        public string NickName {get; set;}
        public int Status {get; set;}
        public string RoleId {get; set;}
        public string Phone {get; set;}
    }
}