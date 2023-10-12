﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Webnovel.API.Databases;

#nullable disable

namespace Webnovel.API.Databases.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20231012051524_AddExceptionLogTable")]
    partial class AddExceptionLogTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("WebNovel.API.Databases.Entities.ExceptionLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(1)
                        .HasComment("Id định danh (khóa chính)");

                    b.Property<string>("Class")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày tạo dữ liệu");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("DelFlag")
                        .HasColumnType("tinyint(1)")
                        .HasComment("Cờ xóa dữ liệu");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày xoá dữ liệu");

                    b.Property<string>("InnerException")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Project")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("StackTrace")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày cập nhật dữ liệu");

                    b.HasKey("Id");

                    b.ToTable("ExceptionLogs");
                });

            modelBuilder.Entity("WebNovel.API.Databases.Entities.Roles", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(21)
                        .HasColumnType("varchar(21)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày tạo dữ liệu");

                    b.Property<bool>("DelFlag")
                        .HasColumnType("tinyint(1)")
                        .HasComment("Cờ xóa dữ liệu");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày xoá dữ liệu");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày cập nhật dữ liệu");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("WebNovel.API.Databases.Entitites.Account", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(1)
                        .HasComment("Id định danh (khóa chính)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày tạo dữ liệu");

                    b.Property<bool>("DelFlag")
                        .HasColumnType("tinyint(1)")
                        .HasComment("Cờ xóa dữ liệu");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày xoá dữ liệu");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsVerifyEmail")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Phone")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("varchar(21)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasComment("Ngày cập nhật dữ liệu");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<float>("WalletAmmount")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("RoleId")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("WebNovel.API.Databases.Entitites.Account", b =>
                {
                    b.HasOne("WebNovel.API.Databases.Entities.Roles", "Role")
                        .WithOne("Account")
                        .HasForeignKey("WebNovel.API.Databases.Entitites.Account", "RoleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("WebNovel.API.Databases.Entities.Roles", b =>
                {
                    b.Navigation("Account")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
