using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bina.Migrations
{
    /// <inheritdoc />
    public partial class quynh01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Articles__Articl__4CA06362",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__Articles__Articl__4D94879B",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__Articles__ImageI__4E88ABD4",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__Articles__UserID__4BAC3F29",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__ArticleSt__Facul__46E78A0C",
                table: "ArticleStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__User__1788CCAC65166875",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK__TermsAnd__C05EBE004AD38F21",
                table: "TermsAndConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__8AFACE3A845AF9EB",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Image__7516F4EC4320BA76",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Faculty__306F636EFFFF63A6",
                table: "Faculty");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleS__3F0E2D6B8284D621",
                table: "ArticleStatus");

            migrationBuilder.DropIndex(
                name: "IX_ArticleStatus_FacultyID",
                table: "ArticleStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__253F2FDC8F761093",
                table: "ArticlesDeadline");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__9C6270C8BC49B6AE",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleC__C3B4DFAA2D5382AB",
                table: "ArticleComments");

            migrationBuilder.DropColumn(
                name: "FacultyID",
                table: "ArticleStatus");

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Faculty",
                type: "tinyint",
                nullable: true,
                defaultValue: (byte)1);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDue",
                table: "ArticlesDeadline",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "ArticlesDeadline",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticlesDeadlineID",
                table: "ArticlesDeadline",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Content",
                table: "Articles",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticlesDeadlineID",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArticleID",
                table: "Articles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "FacultyID",
                table: "Articles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__User__1788CCAC9614F817",
                table: "User",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__TermsAnd__C05EBE007DF42116",
                table: "TermsAndConditions",
                column: "TermsID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__8AFACE3A6187355F",
                table: "Role",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Image__7516F4EC166EBC5D",
                table: "Image",
                column: "ImageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Faculty__306F636EDD7AFA96",
                table: "Faculty",
                column: "FacultyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleS__3F0E2D6BBA8C3447",
                table: "ArticleStatus",
                column: "ArticleStatusID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__253F2FDC87CA24C5",
                table: "ArticlesDeadline",
                column: "ArticlesDeadlineID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__9C6270C84F6C7187",
                table: "Articles",
                column: "ArticleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleC__C3B4DFAA9BFC0A74",
                table: "ArticleComments",
                column: "CommentID");

            migrationBuilder.CreateTable(
                name: "HelpAndSupport",
                columns: table => new
                {
                    HelpSupportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserMessages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HelpAndS__65D53B0F9FCB88E6", x => x.HelpSupportID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_FacultyID",
                table: "Articles",
                column: "FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__Articl__4BAC3F29",
                table: "Articles",
                column: "ArticleStatusID",
                principalTable: "ArticleStatus",
                principalColumn: "ArticleStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__Articl__4CA06362",
                table: "Articles",
                column: "ArticlesDeadlineID",
                principalTable: "ArticlesDeadline",
                principalColumn: "ArticlesDeadlineID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__Facult__4E88ABD4",
                table: "Articles",
                column: "FacultyID",
                principalTable: "Faculty",
                principalColumn: "FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__ImageI__4D94879B",
                table: "Articles",
                column: "ImageID",
                principalTable: "Image",
                principalColumn: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__UserID__4AB81AF0",
                table: "Articles",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Articles__Articl__4BAC3F29",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__Articles__Articl__4CA06362",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__Articles__Facult__4E88ABD4",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__Articles__ImageI__4D94879B",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__Articles__UserID__4AB81AF0",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "HelpAndSupport");

            migrationBuilder.DropPrimaryKey(
                name: "PK__User__1788CCAC9614F817",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK__TermsAnd__C05EBE007DF42116",
                table: "TermsAndConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__8AFACE3A6187355F",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Image__7516F4EC166EBC5D",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Faculty__306F636EDD7AFA96",
                table: "Faculty");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleS__3F0E2D6BBA8C3447",
                table: "ArticleStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__253F2FDC87CA24C5",
                table: "ArticlesDeadline");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__9C6270C84F6C7187",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_FacultyID",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleC__C3B4DFAA9BFC0A74",
                table: "ArticleComments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Faculty");

            migrationBuilder.DropColumn(
                name: "FacultyID",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "FacultyID",
                table: "ArticleStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDue",
                table: "ArticlesDeadline",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DueDate",
                table: "ArticlesDeadline",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArticlesDeadlineID",
                table: "ArticlesDeadline",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Articles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArticlesDeadlineID",
                table: "Articles",
                type: "int",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArticleID",
                table: "Articles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK__User__1788CCAC65166875",
                table: "User",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__TermsAnd__C05EBE004AD38F21",
                table: "TermsAndConditions",
                column: "TermsID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__8AFACE3A845AF9EB",
                table: "Role",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Image__7516F4EC4320BA76",
                table: "Image",
                column: "ImageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Faculty__306F636EFFFF63A6",
                table: "Faculty",
                column: "FacultyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleS__3F0E2D6B8284D621",
                table: "ArticleStatus",
                column: "ArticleStatusID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__253F2FDC8F761093",
                table: "ArticlesDeadline",
                column: "ArticlesDeadlineID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__9C6270C8BC49B6AE",
                table: "Articles",
                column: "ArticleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleC__C3B4DFAA2D5382AB",
                table: "ArticleComments",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleStatus_FacultyID",
                table: "ArticleStatus",
                column: "FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__Articl__4CA06362",
                table: "Articles",
                column: "ArticleStatusID",
                principalTable: "ArticleStatus",
                principalColumn: "ArticleStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__Articl__4D94879B",
                table: "Articles",
                column: "ArticlesDeadlineID",
                principalTable: "ArticlesDeadline",
                principalColumn: "ArticlesDeadlineID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__ImageI__4E88ABD4",
                table: "Articles",
                column: "ImageID",
                principalTable: "Image",
                principalColumn: "ImageID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__UserID__4BAC3F29",
                table: "Articles",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK__ArticleSt__Facul__46E78A0C",
                table: "ArticleStatus",
                column: "FacultyID",
                principalTable: "Faculty",
                principalColumn: "FacultyID");
        }
    }
}
