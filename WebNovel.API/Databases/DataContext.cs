using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Databases;
using WebNovel.API.Databases.Entities;
using WebNovel.API.Databases.Entitites;

namespace Webnovel.API.Databases
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RolesOfUser> RolesOfUsers { get; set; } = null!;
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public virtual DbSet<Novel> Novel { get; set; } = null!;
        public virtual DbSet<Genre> Genre { get; set; } = null!;
        public virtual DbSet<NovelGenre> GenreOfNovels { get; set; }
        public virtual DbSet<Bookmarked> BookMarked { get; set; } = null!;
        public virtual DbSet<Chapter> Chapter { get; set; } = null!;
        public virtual DbSet<Comment> Comment { get; set; } = null!;
        public virtual DbSet<UpdatedFee> UpdatedFee { get; set; } = null!;
        public virtual DbSet<Preferences> Preferences { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;


        /// <summary>
        /// Quy định định dạng dữ liệu và liên kết của các bảng.
        /// <para>Created at: 10/07/2020</para>
        /// <para>Created by: QuyPN</para>
        /// </summary>
        /// <param name="modelBuilder">builder để cấu hình định dạng dữ liệu và liên kết của các bảng</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            QueryFilter.HasQueryFilter(modelBuilder);
            ModelCreate.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Ghi đè phương thức lưu vào DB để lưu thêm các dữ liệu mặc định cần thiết
        /// <para>Created at: 10/07/2020</para>
        /// <para>Created by: QuyPN</para>
        /// </summary>
        /// <returns>
        /// Số lượng record ảnh hưởng
        /// </returns>
        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Lưu thêm các thông tin cần thiết mặc định khi cập nhật dữ diệu vào Database.
        /// <para>Created at: 10/07/2020</para>
        /// <para>Created by: QuyPN</para>
        /// </summary>
        private void OnBeforeSaving()
        {
            // Nếu có sự thay đổi dữ liệu
            if (ChangeTracker.HasChanges())
            {
                // Láy các thông tin cơ bản từ hệ thống
                DateTimeOffset now = DateTimeOffset.Now;
                // Duyệt qua hết tất cả dối tượng có thay đổi
                foreach (var entry in ChangeTracker.Entries())
                {
                    try
                    {
                        if (entry.Entity is Table root)
                        {
                            switch (entry.State)
                            {
                                // Nếu là thêm mới thì cập nhật thông tin thêm mới
                                case EntityState.Added:
                                    {

                                        root.CreatedAt = now;
                                        root.UpdatedAt = null;
                                        root.DelFlag = false;
                                        break;
                                    }
                                // Nếu là update thì cập nhật các trường liên quan đến update
                                case EntityState.Modified:
                                    {
                                        root.UpdatedAt = now;
                                        break;
                                    }
                                case EntityState.Deleted:
                                    {
                                        if (!root.ForceDel)
                                        {
                                            entry.State = EntityState.Modified;
                                            root.DeletedAt = now;
                                            root.DelFlag = true;
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        public async Task RollbackAsync(IDbContextTransaction transaction)
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }
        }

        public void Rollback(IDbContextTransaction transaction)
        {
            if (transaction != null)
            {
                transaction.Rollback();
            }
        }



    }
}