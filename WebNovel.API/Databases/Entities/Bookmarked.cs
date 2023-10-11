using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases.Entities
{
    public class Bookmarked : TableHaveIdInt
    {
        public long AccountId {get; set;}
        public virtual Account Account {get; set;} = null!;
        public long ChapterId {get; set;}
        public virtual Chapter Chapter {get; set;} = null!;
        
    }
}