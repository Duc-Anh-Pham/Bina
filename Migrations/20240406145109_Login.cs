using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bina.Migrations
{
    /// <inheritdoc />
    public partial class Login : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ArticleCo__Artic__52593CB8",
                table: "ArticleComments");

            migrationBuilder.DropForeignKey(
                name: "FK__ArticleCo__UserI__5165187F",
                table: "ArticleComments");

            migrationBuilder.DropForeignKey(
                name: "FK__ArticlesD__UserI__440B1D61",
                table: "ArticlesDeadline");

            migrationBuilder.DropForeignKey(
                name: "FK__ArticleSt__Facul__46E78A0C",
                table: "ArticleStatus");

            migrationBuilder.DropForeignKey(
                name: "FK__User__FacultyID__403A8C7D",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK__User__RoleID__3F466844",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK__User__TermsID__412EB0B6",
                table: "User");

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

            migrationBuilder.RenameColumn(
                name: "UserFullName",
                table: "User",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

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
                name: "PK__User__1788CCAC5AD7973B",
                table: "User",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__TermsAnd__C05EBE00912A5A23",
                table: "TermsAndConditions",
                column: "TermsID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__8AFACE3A7F148AAB",
                table: "Role",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Image__7516F4EC610532C1",
                table: "Image",
                column: "ImageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Faculty__306F636E0C1897AA",
                table: "Faculty",
                column: "FacultyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleS__3F0E2D6B20AE9143",
                table: "ArticleStatus",
                column: "ArticleStatusID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__253F2FDC2821DDB2",
                table: "ArticlesDeadline",
                column: "ArticlesDeadlineID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__9C6270C81F121BB0",
                table: "Articles",
                column: "ArticleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleC__C3B4DFAA55297C62",
                table: "ArticleComments",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_FacultyID",
                table: "Articles",
                column: "FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK__ArticleCo__Artic__534D60F1",
                table: "ArticleComments",
                column: "ArticleID",
                principalTable: "Articles",
                principalColumn: "ArticleID");

            migrationBuilder.AddForeignKey(
                name: "FK__ArticleCo__UserI__52593CB8",
                table: "ArticleComments",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK__Articles__Facult__4F7CD00D",
                table: "Articles",
                column: "FacultyID",
                principalTable: "Faculty",
                principalColumn: "FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK__ArticlesD__UserI__44FF419A",
                table: "ArticlesDeadline",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK__User__FacultyID__412EB0B6",
                table: "User",
                column: "FacultyID",
                principalTable: "Faculty",
                principalColumn: "FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK__User__RoleID__403A8C7D",
                table: "User",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK__User__TermsID__4222D4EF",
                table: "User",
                column: "TermsID",
                principalTable: "TermsAndConditions",
                principalColumn: "TermsID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__ArticleCo__Artic__534D60F1",
                table: "ArticleComments");

            migrationBuilder.DropForeignKey(
                name: "FK__ArticleCo__UserI__52593CB8",
                table: "ArticleComments");

            migrationBuilder.DropForeignKey(
                name: "FK__Articles__Facult__4F7CD00D",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK__ArticlesD__UserI__44FF419A",
                table: "ArticlesDeadline");

            migrationBuilder.DropForeignKey(
                name: "FK__User__FacultyID__412EB0B6",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK__User__RoleID__403A8C7D",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK__User__TermsID__4222D4EF",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK__User__1788CCAC5AD7973B",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK__TermsAnd__C05EBE00912A5A23",
                table: "TermsAndConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__8AFACE3A7F148AAB",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Image__7516F4EC610532C1",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Faculty__306F636E0C1897AA",
                table: "Faculty");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleS__3F0E2D6B20AE9143",
                table: "ArticleStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__253F2FDC2821DDB2",
                table: "ArticlesDeadline");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__9C6270C81F121BB0",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_FacultyID",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleC__C3B4DFAA55297C62",
                table: "ArticleComments");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Faculty");

            migrationBuilder.DropColumn(
                name: "FacultyID",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "User",
                newName: "UserFullName");

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
                name: "FK__ArticleCo__Artic__52593CB8",
                table: "ArticleComments",
                column: "ArticleID",
                principalTable: "Articles",
                principalColumn: "ArticleID");

            migrationBuilder.AddForeignKey(
                name: "FK__ArticleCo__UserI__5165187F",
                table: "ArticleComments",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK__ArticlesD__UserI__440B1D61",
                table: "ArticlesDeadline",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK__ArticleSt__Facul__46E78A0C",
                table: "ArticleStatus",
                column: "FacultyID",
                principalTable: "Faculty",
                principalColumn: "FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK__User__FacultyID__403A8C7D",
                table: "User",
                column: "FacultyID",
                principalTable: "Faculty",
                principalColumn: "FacultyID");

            migrationBuilder.AddForeignKey(
                name: "FK__User__RoleID__3F466844",
                table: "User",
                column: "RoleID",
                principalTable: "Role",
                principalColumn: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK__User__TermsID__412EB0B6",
                table: "User",
                column: "TermsID",
                principalTable: "TermsAndConditions",
                principalColumn: "TermsID");
        }
    }
}
