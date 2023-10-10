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
        public string Email {get; set;} = null!;
        public long RoleId {get; set;}
        public virtual Roles Role {get; set;}
        [StringLength(100)]
        public string NickName {get; set;} = null!;
        [DataType(DataType.Date)]
        public DateOnly DateJoined {get; set;}
        public bool Status {get; set;}
        public float WalletAmmount {get; set;} = 0.0f;

    }
}