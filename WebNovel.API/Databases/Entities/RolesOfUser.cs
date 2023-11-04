using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class RolesOfUser : Table
    {
        public RolesOfUser()
        {
            ForceDel = true;
        }

        [Key]
        [Column(Order = 1)]
        [StringLength(21)]
        public string AccountId { set; get; }

        [Key]
        [Column(Order = 2)]
        [StringLength(21)]
        public string RoleId { set; get; }

        public virtual Account Account { set; get; }

        public virtual Role Role { set; get; }

    }
}