using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webnovel.API.Migrations
{
    /// <inheritdoc />
    public partial class StringIdLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "Ratings",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Ratings",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "Preferences",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Preferences",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Novel",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Novel",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                comment: "Id định danh (khóa chính)",
                oldClrType: typeof(string),
                oldType: "varchar(21)",
                oldMaxLength: 21,
                oldComment: "Id định danh (khóa chính)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "GenreOfNovels",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Comment",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "Comment",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "Chapter",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Chapter",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                comment: "Id định danh (khóa chính)",
                oldClrType: typeof(string),
                oldType: "varchar(21)",
                oldMaxLength: 21,
                oldComment: "Id định danh (khóa chính)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ChapterId",
                table: "BookMarked",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "BookMarked",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Accounts",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                comment: "Id định danh (khóa chính)",
                oldClrType: typeof(string),
                oldType: "varchar(21)",
                oldMaxLength: 21,
                oldComment: "Id định danh (khóa chính)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "Ratings",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Ratings",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "Preferences",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Preferences",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Novel",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Novel",
                type: "varchar(21)",
                maxLength: 21,
                nullable: false,
                comment: "Id định danh (khóa chính)",
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36,
                oldComment: "Id định danh (khóa chính)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "GenreOfNovels",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Comment",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "Comment",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "NovelId",
                table: "Chapter",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Chapter",
                type: "varchar(21)",
                maxLength: 21,
                nullable: false,
                comment: "Id định danh (khóa chính)",
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36,
                oldComment: "Id định danh (khóa chính)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ChapterId",
                table: "BookMarked",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "BookMarked",
                type: "varchar(21)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Accounts",
                type: "varchar(21)",
                maxLength: 21,
                nullable: false,
                comment: "Id định danh (khóa chính)",
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldMaxLength: 36,
                oldComment: "Id định danh (khóa chính)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
