using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Databases.Entities
{
    public class AccountRole
    {
        public long AccountId {get; set;}
        public string RoleId {get; set;} = null!;

    }
}