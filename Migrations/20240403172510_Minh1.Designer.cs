﻿// <auto-generated />
using System;
using Bina.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Bina.Migrations
{
    [DbContext(typeof(Ft1Context))]
    [Migration("20240403172510_Minh1")]
    partial class Minh1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<byte[]>("Content")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FacultyId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("FacultyID");

                    b.Property<int?>("ImageId")
                        .HasColumnType("int")
                        .HasColumnName("ImageID");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ArticleId")
                        .HasName("PK__Articles__9C6270C83AEB18D4");

                    b.HasIndex("ArticleStatusId");

                    b.HasIndex("ArticlesDeadlineId");

                    b.HasIndex("FacultyId");

                    b.HasIndex("ImageId");

                    b.HasIndex("UserId");

                    b.ToTable("Articles");
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
                        .HasName("PK__ArticleC__C3B4DFAA1BF3DCB0");

                    b.HasIndex("ArticleId");

                    b.HasIndex("UserId");

                    b.ToTable("ArticleComments");
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
                        .HasName("PK__ArticleS__3F0E2D6B56B8CFEF");

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

                    b.Property<DateTime?>("StartDue")
                        .HasColumnType("datetime");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ArticlesDeadlineId")
                        .HasName("PK__Articles__253F2FDCC4C11E50");

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

                    b.HasKey("FacultyId")
                        .HasName("PK__Faculty__306F636E7A568F48");

                    b.ToTable("Faculty", (string)null);
                });

            modelBuilder.Entity("Bina.Models.Image", b =>
                {
                    b.Property<int>("ImageId")
                        .HasColumnType("int")
                        .HasColumnName("ImageID");

                    b.Property<string>("Imagepath")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.HasKey("ImageId")
                        .HasName("PK__Image__7516F4EC8406AB4E");

                    b.ToTable("Image", (string)null);
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
                        .HasName("PK__Role__8AFACE3A7CC19ACD");

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
                        .HasName("PK__TermsAnd__C05EBE008C57B0FF");

                    b.ToTable("TermsAndConditions");
                });

            modelBuilder.Entity("Bina.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

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

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Password")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("PhoneNumber")
                        .HasColumnType("int");

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

                    b.Property<string>("UserFullName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UserId")
                        .HasName("PK__User__1788CCAC118B22FA");

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
                        .HasConstraintName("FK__Articles__Articl__38996AB5");

                    b.HasOne("Bina.Models.ArticlesDeadline", "ArticlesDeadline")
                        .WithMany("Articles")
                        .HasForeignKey("ArticlesDeadlineId")
                        .HasConstraintName("FK__Articles__Articl__398D8EEE");

                    b.HasOne("Bina.Models.Faculty", "Faculty")
                        .WithMany("Articles")
                        .HasForeignKey("FacultyId")
                        .HasConstraintName("FK__Articles__Facult__3B75D760");

                    b.HasOne("Bina.Models.Image", "Image")
                        .WithMany("Articles")
                        .HasForeignKey("ImageId")
                        .HasConstraintName("FK__Articles__ImageI__3A81B327");

                    b.HasOne("Bina.Models.User", "User")
                        .WithMany("Articles")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Articles__UserID__37A5467C");

                    b.Navigation("ArticleStatus");

                    b.Navigation("ArticlesDeadline");

                    b.Navigation("Faculty");

                    b.Navigation("Image");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Bina.Models.ArticleComment", b =>
                {
                    b.HasOne("Bina.Models.Article", "Article")
                        .WithMany("ArticleComments")
                        .HasForeignKey("ArticleId")
                        .HasConstraintName("FK__ArticleCo__Artic__3F466844");

                    b.HasOne("Bina.Models.User", "User")
                        .WithMany("ArticleComments")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__ArticleCo__UserI__3E52440B");

                    b.Navigation("Article");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Bina.Models.ArticlesDeadline", b =>
                {
                    b.HasOne("Bina.Models.User", "User")
                        .WithMany("ArticlesDeadlines")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__ArticlesD__UserI__30F848ED");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Bina.Models.User", b =>
                {
                    b.HasOne("Bina.Models.Faculty", "Faculty")
                        .WithMany("Users")
                        .HasForeignKey("FacultyId")
                        .HasConstraintName("FK__User__FacultyID__2D27B809");

                    b.HasOne("Bina.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__User__RoleID__2C3393D0");

                    b.HasOne("Bina.Models.TermsAndCondition", "Terms")
                        .WithMany("Users")
                        .HasForeignKey("TermsId")
                        .HasConstraintName("FK__User__TermsID__2E1BDC42");

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

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Bina.Models.Image", b =>
                {
                    b.Navigation("Articles");
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
