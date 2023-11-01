using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webnovel.API.Databases.Migrations
{
    /// <inheritdoc />
    public partial class ImplementRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    NovelId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    RateScore = table.Column<float>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Ngày tạo dữ liệu"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày cập nhật dữ liệu"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "Ngày xoá dữ liệu"),
                    DelFlag = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Cờ xóa dữ liệu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => new { x.AccountId, x.NovelId });
                    table.ForeignKey(
                        name: "FK_Ratings_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ratings_Novel_NovelId",
                        column: x => x.NovelId,
                        principalTable: "Novel",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_NovelId",
                table: "Ratings",
                column: "NovelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ratings");
        }
    }
}
