using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebNovel.API.Databases
{
    public class QueryFilter
    {
        public static ModelBuilder HasQueryFilter(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasQueryFilter(x => !x.DelFlag);

            return modelBuilder;
        }
    }
}