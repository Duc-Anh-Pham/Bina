﻿// <auto-generated />
using System;
using Bina.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Bina.Migrations
{
    [DbContext(typeof(Ft1Context))]
    partial class Ft1ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bina.Models.Article", b =>
                {
                    b.Property<int>("ArticleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ArticleID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ArticleId"));

                    b.Property<string>("ArticleName")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("ArticleStatusId")
                        .HasColumnType("int")
                        .HasColumnName("ArticleStatusID");

                    b.Property<Guid?>("ArticlesDeadlineId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ArticlesDeadlineID");

                    b.Property<string>("Content")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("DocumentPath")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FacultyId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("FacultyID");

                    b.Property<string>("ImagePath")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ArticleId")
                        .HasName("PK__Articles__9C6270C8CB56DD9F");

                    b.HasIndex("ArticleStatusId");

                    b.HasIndex("ArticlesDeadlineId");

                    b.HasIndex("FacultyId");

                    b.HasIndex("UserId");

                    b.ToTable("Articles", (string)null);
                });

            modelBuilder.Entity("Bina.Models.ArticleComment", b =>
                {
                    b.Property<int>("CommentId")
                        .HasColumnType("int")
                        .HasColumnName("CommentID");

                    b.Property<int?>("ArticleId")
                        .HasColumnType("int")
                        .HasColumnName("ArticleID");

                    b.Property<DateOnly?>("CommentDay")
                        .HasColumnType("date");

                    b.Property<string>("CommentText")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("CommentId")
                        .HasName("PK__ArticleC__C3B4DFAA15BFFD4E");

                    b.HasIndex("ArticleId");

                    b.HasIndex("UserId");

                    b.ToTable("ArticleComments", (string)null);
                });

            modelBuilder.Entity("Bina.Models.ArticleStatus", b =>
                {
                    b.Property<int>("ArticleStatusId")
                        .HasColumnType("int")
                        .HasColumnName("ArticleStatusID");

                    b.Property<string>("ArticleStatusName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ArticleStatusId")
                        .HasName("PK__ArticleS__3F0E2D6B45808540");

                    b.ToTable("ArticleStatus", (string)null);
                });

            modelBuilder.Entity("Bina.Models.ArticlesDeadline", b =>
                {
                    b.Property<Guid>("ArticlesDeadlineId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ArticlesDeadlineID");

                    b.Property<int?>("AcademicYear")
                        .HasColumnType("int")
                        .HasColumnName("academicYear");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime");

                    b.Property<string>("FacultyId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("FacultyID");

                    b.Property<DateTime?>("StartDue")
                        .HasColumnType("datetime");

                    b.Property<string>("TermName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TermTitle")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ArticlesDeadlineId")
                        .HasName("PK__Articles__253F2FDCCAA1AF57");

                    b.HasIndex("FacultyId");

                    b.HasIndex("UserId");

                    b.ToTable("ArticlesDeadline", (string)null);
                });

            modelBuilder.Entity("Bina.Models.Faculty", b =>
                {
                    b.Property<string>("FacultyId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("FacultyID");

                    b.Property<DateTime?>("Established")
                        .HasColumnType("datetime");

                    b.Property<string>("FacultyName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte?>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasDefaultValue((byte)1);

                    b.HasKey("FacultyId")
                        .HasName("PK__Faculty__306F636ED538B474");

                    b.ToTable("Faculty", (string)null);
                });

            modelBuilder.Entity("Bina.Models.HelpAndSupport", b =>
                {
                    b.Property<int>("HelpSupportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("HelpSupportID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HelpSupportId"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserMessages")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HelpSupportId")
                        .HasName("PK__HelpAndS__65D53B0F8821C824");

                    b.ToTable("HelpAndSupport", (string)null);
                });

            modelBuilder.Entity("Bina.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    b.Property<string>("RoleName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RoleId")
                        .HasName("PK__Role__8AFACE3AB4D1A7C8");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("Bina.Models.TermsAndCondition", b =>
                {
                    b.Property<int>("TermsId")
                        .HasColumnType("int")
                        .HasColumnName("TermsID");

                    b.Property<string>("TermsText")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("TermsId")
                        .HasName("PK__TermsAnd__C05EBE00C5C4011E");

                    b.ToTable("TermsAndConditions", (string)null);
                });

            modelBuilder.Entity("Bina.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("ConfirmPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateOnly?>("DoB")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FacultyId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("FacultyID");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NewPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OldPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("PhoneNumber")
                        .HasColumnType("int");

                    b.Property<bool>("RememberMe")
                        .HasColumnType("bit");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleID");

                    b.Property<byte?>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasDefaultValue((byte)1);

                    b.Property<int?>("TermsId")
                        .HasColumnType("int")
                        .HasColumnName("TermsID");

                    b.Property<string>("UserName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UserId")
                        .HasName("PK__User__1788CCACF2EED1F7");

                    b.HasIndex("FacultyId");

                    b.HasIndex("RoleId");

                    b.HasIndex("TermsId");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("Bina.Models.Article", b =>
                {
                    b.HasOne("Bina.Models.ArticleStatus", "ArticleStatus")
                        .WithMany("Articles")
                        .HasForeignKey("ArticleStatusId")
                        .HasConstraintName("FK__Articles__Articl__4E88ABD4");

                    b.HasOne("Bina.Models.ArticlesDeadline", "ArticlesDeadline")
                        .WithMany("Articles")
                        .HasForeignKey("ArticlesDeadlineId")
                        .HasConstraintName("FK__Articles__Articl__4F7CD00D");

                    b.HasOne("Bina.Models.Faculty", "Faculty")
                        .WithMany("Articles")
                        .HasForeignKey("FacultyId")
                        .HasConstraintName("FK__Articles__Facult__5070F446");

                    b.HasOne("Bina.Models.User", "User")
                        .WithMany("Articles")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Articles__UserID__4D94879B");

                    b.Navigation("ArticleStatus");

                    b.Navigation("ArticlesDeadline");

                    b.Navigation("Faculty");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Bina.Models.ArticleComment", b =>
                {
                    b.HasOne("Bina.Models.Article", "Article")
                        .WithMany("ArticleComments")
                        .HasForeignKey("ArticleId")
                        .HasConstraintName("FK__ArticleCo__Artic__5441852A");

                    b.HasOne("Bina.Models.User", "User")
                        .WithMany("ArticleComments")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__ArticleCo__UserI__534D60F1");

                    b.Navigation("Article");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Bina.Models.ArticlesDeadline", b =>
                {
                    b.HasOne("Bina.Models.Faculty", "Faculty")
                        .WithMany("ArticlesDeadlines")
                        .HasForeignKey("FacultyId")
                        .HasConstraintName("FK__ArticlesD__Facul__48CFD27E");

                    b.HasOne("Bina.Models.User", "User")
                        .WithMany("ArticlesDeadlines")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__ArticlesD__UserI__47DBAE45");

                    b.Navigation("Faculty");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Bina.Models.User", b =>
                {
                    b.HasOne("Bina.Models.Faculty", "Faculty")
                        .WithMany("Users")
                        .HasForeignKey("FacultyId")
                        .HasConstraintName("FK__User__FacultyID__412EB0B6");

                    b.HasOne("Bina.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__User__RoleID__403A8C7D");

                    b.HasOne("Bina.Models.TermsAndCondition", "Terms")
                        .WithMany("Users")
                        .HasForeignKey("TermsId")
                        .HasConstraintName("FK__User__TermsID__4222D4EF");

                    b.Navigation("Faculty");

                    b.Navigation("Role");

                    b.Navigation("Terms");
                });

            modelBuilder.Entity("Bina.Models.Article", b =>
                {
                    b.Navigation("ArticleComments");
                });

            modelBuilder.Entity("Bina.Models.ArticleStatus", b =>
                {
                    b.Navigation("Articles");
                });

            modelBuilder.Entity("Bina.Models.ArticlesDeadline", b =>
                {
                    b.Navigation("Articles");
                });

            modelBuilder.Entity("Bina.Models.Faculty", b =>
                {
                    b.Navigation("Articles");

                    b.Navigation("ArticlesDeadlines");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Bina.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Bina.Models.TermsAndCondition", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Bina.Models.User", b =>
                {
                    b.Navigation("ArticleComments");

                    b.Navigation("Articles");

                    b.Navigation("ArticlesDeadlines");
                });
#pragma warning restore 612, 618
        }
    }
}
