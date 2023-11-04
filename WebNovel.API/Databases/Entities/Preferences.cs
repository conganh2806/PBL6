using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class Preferences :Table
    {
        public long NovelId {get; set;}
        public string AccountId {get; set;}

        public virtual Novel Novel {get; set;} = null!;
        public virtual Account Account {get; set;} = null!;

    }
}