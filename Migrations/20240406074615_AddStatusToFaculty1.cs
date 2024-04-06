using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bina.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToFaculty1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__User__1788CCAC118B22FA",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK__TermsAnd__C05EBE008C57B0FF",
                table: "TermsAndConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__8AFACE3A7CC19ACD",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Image__7516F4EC8406AB4E",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Faculty__306F636E7A568F48",
                table: "Faculty");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleS__3F0E2D6B56B8CFEF",
                table: "ArticleStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__253F2FDCC4C11E50",
                table: "ArticlesDeadline");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__9C6270C83AEB18D4",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleC__C3B4DFAA1BF3DCB0",
                table: "ArticleComments");

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "Faculty",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK__User__1788CCACB81EF04B",
                table: "User",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__TermsAnd__C05EBE000B28BBAA",
                table: "TermsAndConditions",
                column: "TermsID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__8AFACE3A272CC538",
                table: "Role",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Image__7516F4EC437AB771",
                table: "Image",
                column: "ImageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Faculty__306F636EB2995065",
                table: "Faculty",
                column: "FacultyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleS__3F0E2D6BF59EE711",
                table: "ArticleStatus",
                column: "ArticleStatusID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__253F2FDCBC23781D",
                table: "ArticlesDeadline",
                column: "ArticlesDeadlineID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__9C6270C85CA7DDC9",
                table: "Articles",
                column: "ArticleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleC__C3B4DFAAF65DE434",
                table: "ArticleComments",
                column: "CommentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK__User__1788CCACB81EF04B",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK__TermsAnd__C05EBE000B28BBAA",
                table: "TermsAndConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__8AFACE3A272CC538",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Image__7516F4EC437AB771",
                table: "Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Faculty__306F636EB2995065",
                table: "Faculty");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleS__3F0E2D6BF59EE711",
                table: "ArticleStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__253F2FDCBC23781D",
                table: "ArticlesDeadline");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Articles__9C6270C85CA7DDC9",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK__ArticleC__C3B4DFAAF65DE434",
                table: "ArticleComments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Faculty");

            migrationBuilder.AddPrimaryKey(
                name: "PK__User__1788CCAC118B22FA",
                table: "User",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__TermsAnd__C05EBE008C57B0FF",
                table: "TermsAndConditions",
                column: "TermsID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__8AFACE3A7CC19ACD",
                table: "Role",
                column: "RoleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Image__7516F4EC8406AB4E",
                table: "Image",
                column: "ImageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Faculty__306F636E7A568F48",
                table: "Faculty",
                column: "FacultyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleS__3F0E2D6B56B8CFEF",
                table: "ArticleStatus",
                column: "ArticleStatusID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__253F2FDCC4C11E50",
                table: "ArticlesDeadline",
                column: "ArticlesDeadlineID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Articles__9C6270C83AEB18D4",
                table: "Articles",
                column: "ArticleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__ArticleC__C3B4DFAA1BF3DCB0",
                table: "ArticleComments",
                column: "CommentID");
        }
    }
}
