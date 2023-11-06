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
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Role>()
            .HasMany(e => e.Accounts)
            .WithOne(e => e.Role)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
            .Entity<RolesOfUser>()
            .HasKey(e => new
            {
                e.AccountId,
                e.RoleId
            });

            modelBuilder.Entity<Account>()
            .HasMany(e => e.Preferences)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Novel>()
            .HasMany(e => e.Preferences)
            .WithOne(e => e.Novel)
            .HasForeignKey(e => e.NovelId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
            .Entity<Preferences>()
            .HasKey(e => new
            {
                e.AccountId,
                e.NovelId
            });

            modelBuilder.Entity<Novel>()
            .HasMany(n => n.Genres)
            .WithOne(g => g.Novel)
            .HasForeignKey(e => e.NovelId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Genre>()
            .HasMany(n => n.Novels)
            .WithOne(g => g.Genre)
            .HasForeignKey(e => e.GenreId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
            .Entity<NovelGenre>()
            .HasKey(e => new
            {
                e.GenreId,
                e.NovelId
            });

            modelBuilder.Entity<Comment>()
            .HasOne(e => e.Novel)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.NovelId)
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

            modelBuilder.Entity<UpdatedFee>()
            .HasMany(e => e.Chapters)
            .WithOne(e => e.UpdatedFee)
            .HasForeignKey(e => e.FeeId);

            modelBuilder.Entity<Novel>()
            .HasMany(e => e.Bookmarkeds)
            .WithOne(e => e.Novel)
            .HasForeignKey(e => e.NovelId)
            .IsRequired();

            modelBuilder.Entity<Account>()
            .HasMany(e => e.Bookmarkeds)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();

            modelBuilder.Entity<Chapter>()
            .HasMany(e => e.Bookmarked)
            .WithOne(e => e.Chapter)
            .HasForeignKey(e => e.ChapterId)
            .IsRequired();

            modelBuilder.Entity<Bookmarked>()
            .HasKey(e => new
            {
                e.AccountId,
                e.NovelId,
            });

            modelBuilder.Entity<Account>()
            .HasMany(e => e.Preferences)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();

            modelBuilder.Entity<Account>()
            .HasMany(e => e.Ratings)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Novel>()
            .HasMany(e => e.Ratings)
            .WithOne(e => e.Novel)
            .HasForeignKey(e => e.NovelId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
            .Entity<Rating>()
            .HasKey(e => new
            {
                e.AccountId,
                e.NovelId
            });



            return modelBuilder;
        }
    }
}