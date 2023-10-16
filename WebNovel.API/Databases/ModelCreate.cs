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
            .HasMany(e => e.Roles)
            .WithMany(e => e.Accounts)
            .UsingEntity<AccountRole>();
            
            modelBuilder.Entity<Account>()
            .HasMany(e => e.Novels)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId);

            modelBuilder.Entity<Novel>()
            .HasMany<Genre>(n => n.Genres)
            .WithMany(g => g.Novels)
            .UsingEntity<NovelGenre>();

            modelBuilder.Entity<Comment>()
            .HasOne(e => e.Novel)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.Id)
            .IsRequired();

            modelBuilder.Entity<Comment>()
            .HasOne(e => e.Account)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();

            modelBuilder.Entity<Chapter>()
            .HasOne(e => e.Novel)
            .WithMany(e => e.Chapters)
            .HasForeignKey(e => e.NovelId);

            modelBuilder.Entity<Chapter>()
            .HasOne(e => e.UpdatedFee)
            .WithOne(e => e.Chapter)
            .HasForeignKey<Chapter>(e => e.FeeId);

            modelBuilder.Entity<Chapter>()
            .HasOne(e => e.Bookmarked)
            .WithOne(e => e.Chapter)
            .HasForeignKey<Bookmarked>(e => e.ChapterId)
            .IsRequired();

            modelBuilder.Entity<Account>()
            .HasMany(e => e.Bookmarkeds)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();

            modelBuilder.Entity<Account>()
            .HasMany(e => e.Preferences)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();

            modelBuilder.Entity<Novel>()
            .HasMany(e => e.Preferences)
            .WithOne(e => e.Novel)
            .HasForeignKey(e => e.NovelId)
            .IsRequired();


           
            return modelBuilder;
        }
    }
}