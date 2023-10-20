using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webnovel.API.Databases.Migrations
{
    /// <inheritdoc />
    public partial class FixDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id định danh (khóa chính)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NickName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateJoined = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WalletAmmount = table.Column<float>(type: "float", nullable: false),
                    IsVerifyEmail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExceptionLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id định danh (khóa chính)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Project = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Class = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Method = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Data = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StackTrace = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InnerException = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id định danh (khóa chính)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UpdatedFee",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id định danh (khóa chính)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fee = table.Column<float>(type: "float", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdatedFee", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Novel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id định danh (khóa chính)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Images = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ApprovalStatus = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Novel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Novel_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolesOfUsers",
                columns: table => new
                {
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesOfUsers", x => new { x.AccountId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RolesOfUsers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RolesOfUsers_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Chapter",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id định danh (khóa chính)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    FeeId = table.Column<long>(type: "bigint", nullable: false),
                    FileContent = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    ApprovalStatus = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NovelId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapter_Novel_NovelId",
                        column: x => x.NovelId,
                        principalTable: "Novel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Chapter_UpdatedFee_FeeId",
                        column: x => x.FeeId,
                        principalTable: "UpdatedFee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id định danh (khóa chính)"),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    NovelId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_Novel_Id",
                        column: x => x.Id,
                        principalTable: "Novel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GenreOfNovels",
                columns: table => new
                {
                    NovelId = table.Column<long>(type: "bigint", nullable: false),
                    GenreId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreOfNovels", x => new { x.GenreId, x.NovelId });
                    table.ForeignKey(
                        name: "FK_GenreOfNovels_Genre_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genre",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GenreOfNovels_Novel_NovelId",
                        column: x => x.NovelId,
                        principalTable: "Novel",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    NovelId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => new { x.AccountId, x.NovelId });
                    table.ForeignKey(
                        name: "FK_Preferences_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Preferences_Novel_NovelId",
                        column: x => x.NovelId,
                        principalTable: "Novel",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BookMarked",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id định danh (khóa chính)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    ChapterId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookMarked", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookMarked_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookMarked_Chapter_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BookMarked_AccountId",
                table: "BookMarked",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BookMarked_ChapterId",
                table: "BookMarked",
                column: "ChapterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_FeeId",
                table: "Chapter",
                column: "FeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_NovelId",
                table: "Chapter",
                column: "NovelId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AccountId",
                table: "Comment",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreOfNovels_NovelId",
                table: "GenreOfNovels",
                column: "NovelId");

            migrationBuilder.CreateIndex(
                name: "IX_Novel_AccountId",
                table: "Novel",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Preferences_NovelId",
                table: "Preferences",
                column: "NovelId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesOfUsers_RoleId",
                table: "RolesOfUsers",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookMarked");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "ExceptionLogs");

            migrationBuilder.DropTable(
                name: "GenreOfNovels");

            migrationBuilder.DropTable(
                name: "Preferences");

            migrationBuilder.DropTable(
                name: "RolesOfUsers");

            migrationBuilder.DropTable(
                name: "Chapter");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Novel");

            migrationBuilder.DropTable(
                name: "UpdatedFee");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
