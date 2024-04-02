using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bina.Migrations
{
    /// <inheritdoc />
    public partial class add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faculty",
                columns: table => new
                {
                    FacultyID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FacultyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Established = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Faculty__306F636EFFFF63A6", x => x.FacultyID);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    ImageID = table.Column<int>(type: "int", nullable: false),
                    Imagepath = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Image__7516F4EC4320BA76", x => x.ImageID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE3A845AF9EB", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "TermsAndConditions",
                columns: table => new
                {
                    TermsID = table.Column<int>(type: "int", nullable: false),
                    TermsText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TermsAnd__C05EBE004AD38F21", x => x.TermsID);
                });

            migrationBuilder.CreateTable(
                name: "ArticleStatus",
                columns: table => new
                {
                    ArticleStatusID = table.Column<int>(type: "int", nullable: false),
                    ArticleStatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FacultyID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ArticleS__3F0E2D6B8284D621", x => x.ArticleStatusID);
                    table.ForeignKey(
                        name: "FK__ArticleSt__Facul__46E78A0C",
                        column: x => x.FacultyID,
                        principalTable: "Faculty",
                        principalColumn: "FacultyID");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserFullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<int>(type: "int", nullable: true),
                    DoB = table.Column<DateOnly>(type: "date", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    FacultyID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)1),
                    TermsID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__1788CCAC65166875", x => x.UserID);
                    table.ForeignKey(
                        name: "FK__User__FacultyID__403A8C7D",
                        column: x => x.FacultyID,
                        principalTable: "Faculty",
                        principalColumn: "FacultyID");
                    table.ForeignKey(
                        name: "FK__User__RoleID__3F466844",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "FK__User__TermsID__412EB0B6",
                        column: x => x.TermsID,
                        principalTable: "TermsAndConditions",
                        principalColumn: "TermsID");
                });

            migrationBuilder.CreateTable(
                name: "ArticlesDeadline",
                columns: table => new
                {
                    ArticlesDeadlineID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    StartDue = table.Column<DateOnly>(type: "date", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    academicYear = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Articles__253F2FDC8F761093", x => x.ArticlesDeadlineID);
                    table.ForeignKey(
                        name: "FK__ArticlesD__UserI__440B1D61",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ArticleID = table.Column<int>(type: "int", nullable: false),
                    ArticleName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    ImageID = table.Column<int>(type: "int", nullable: true),
                    ArticleStatusID = table.Column<int>(type: "int", nullable: true),
                    ArticlesDeadlineID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Articles__9C6270C8BC49B6AE", x => x.ArticleID);
                    table.ForeignKey(
                        name: "FK__Articles__Articl__4CA06362",
                        column: x => x.ArticleStatusID,
                        principalTable: "ArticleStatus",
                        principalColumn: "ArticleStatusID");
                    table.ForeignKey(
                        name: "FK__Articles__Articl__4D94879B",
                        column: x => x.ArticlesDeadlineID,
                        principalTable: "ArticlesDeadline",
                        principalColumn: "ArticlesDeadlineID");
                    table.ForeignKey(
                        name: "FK__Articles__ImageI__4E88ABD4",
                        column: x => x.ImageID,
                        principalTable: "Image",
                        principalColumn: "ImageID");
                    table.ForeignKey(
                        name: "FK__Articles__UserID__4BAC3F29",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "ArticleComments",
                columns: table => new
                {
                    CommentID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    ArticleID = table.Column<int>(type: "int", nullable: true),
                    CommentDay = table.Column<DateOnly>(type: "date", nullable: true),
                    CommentText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ArticleC__C3B4DFAA2D5382AB", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK__ArticleCo__Artic__52593CB8",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ArticleID");
                    table.ForeignKey(
                        name: "FK__ArticleCo__UserI__5165187F",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleComments_ArticleID",
                table: "ArticleComments",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleComments_UserID",
                table: "ArticleComments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticlesDeadlineID",
                table: "Articles",
                column: "ArticlesDeadlineID");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleStatusID",
                table: "Articles",
                column: "ArticleStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ImageID",
                table: "Articles",
                column: "ImageID");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UserID",
                table: "Articles",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlesDeadline_UserID",
                table: "ArticlesDeadline",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleStatus_FacultyID",
                table: "ArticleStatus",
                column: "FacultyID");

            migrationBuilder.CreateIndex(
                name: "IX_User_FacultyID",
                table: "User",
                column: "FacultyID");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleID",
                table: "User",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_User_TermsID",
                table: "User",
                column: "TermsID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleComments");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "ArticleStatus");

            migrationBuilder.DropTable(
                name: "ArticlesDeadline");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Faculty");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "TermsAndConditions");
        }
    }
}
