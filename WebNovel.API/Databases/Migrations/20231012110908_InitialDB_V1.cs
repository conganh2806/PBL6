using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webnovel.API.Databases.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB_V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Novel_NovelId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_NovelId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ChapterId",
                table: "Chapter");

            migrationBuilder.DropColumn(
                name: "NovelId",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "FileContent",
                table: "Chapter",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Chapter");

            migrationBuilder.AddColumn<int>(
                name: "ChapterId",
                table: "Chapter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "NovelId",
                table: "Accounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_NovelId",
                table: "Accounts",
                column: "NovelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Novel_NovelId",
                table: "Accounts",
                column: "NovelId",
                principalTable: "Novel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
