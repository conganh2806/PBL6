using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebNovel.API.Databases.Entities;
using WebNovel.API.Databases.Entitites;

namespace WebNovel.API.Databases
{
    public class ModelCreate
    {
        public static ModelBuilder OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
            .HasOne(e => e.Role)
            .WithOne(e => e.Account)
            .HasForeignKey<Account>(e => e.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
            return modelBuilder;
        }
    }
}