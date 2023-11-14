using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Databases.Entities
{
    public class IncreasePercent : TableHaveIdInt
    {
        public float Percent {get; set;}
        public int Year {get; set;}

    }
}