using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebNovel.API.Databases.Entitites;


namespace WebNovel.API.Databases.Entities
{
    public class Role : Table
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        [Key]
        [StringLength(21)]
        public string Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;
        public virtual ICollection<Account> Accounts { get; set; } = null!;

    }
}