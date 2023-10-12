using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Databases.Entitites
{
    public class Account : TableHaveIdInt
    {

        [StringLength(100)]
        public string Username {get; set;} = null!;
        [StringLength(255)]
        public string Password {get; set;} = null!;
        [StringLength(255)]
        public string? Email {get; set;}
        [StringLength(21)]
        public string RoleId {get; set;} = null!;
        public virtual Roles Role {get; set;}
        [StringLength(100)]
        public string NickName {get; set;} = null!;
        public int Status {get; set;}
        public float WalletAmmount {get; set;} = 0.0f;
        public bool IsVerifyEmail {get; set;}
        public bool IsActive {get; set;}
        public bool IsAdmin {get; set;}
        public string? Phone {get; set;}
    }
}