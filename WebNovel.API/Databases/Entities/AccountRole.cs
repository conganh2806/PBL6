using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class AccountRole
    {
        [Key]
        [Column(Order = 1)]
        public string AccountId {get; set;}
        [Key]
        [Column(Order = 2)]
        public string RoleId {get; set;} = null!;
        
        public virtual Account Account { set; get; }

        public virtual Role Role { set; get; }

    }
}