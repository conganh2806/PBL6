using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebNovel.API.Databases
{
    public class ModelCreate
    {
        public static ModelBuilder OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<User>()
            // .HasMany(e => e.Addresses)
            // .WithOne(e => e.Commune)
            // .HasForeignKey(e => e.CommuneId)
            // .OnDelete(DeleteBehavior.NoAction);
            return modelBuilder;
        }
    }
}