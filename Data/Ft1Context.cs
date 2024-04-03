using Bina.Models;
using Microsoft.EntityFrameworkCore;

namespace Bina.Data;

public partial class Ft1Context : DbContext
{
    public Ft1Context()
    {
    }

    public Ft1Context(DbContextOptions<Ft1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleComment> ArticleComments { get; set; }

    public virtual DbSet<ArticleStatus> ArticleStatuses { get; set; }

    public virtual DbSet<ArticlesDeadline> ArticlesDeadlines { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TermsAndCondition> TermsAndConditions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-7PT7B3J1;Initial Catalog=FT1;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270C861E9F2B8");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.ArticleName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ArticleStatusId).HasColumnName("ArticleStatusID");
            entity.Property(e => e.ArticlesDeadlineId).HasColumnName("ArticlesDeadlineID");
            entity.Property(e => e.FacultyId)
                .HasMaxLength(50)
                .HasColumnName("FacultyID");
            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ArticleStatus).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ArticleStatusId)
                .HasConstraintName("FK__Articles__Articl__4CA06362");

            entity.HasOne(d => d.ArticlesDeadline).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ArticlesDeadlineId)
                .HasConstraintName("FK__Articles__Articl__4D94879B");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Articles)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__Articles__Facult__4F7CD00D");

            entity.HasOne(d => d.Image).WithMany(p => p.Articles)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK__Articles__ImageI__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.Articles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Articles__UserID__4BAC3F29");
        });

        modelBuilder.Entity<ArticleComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__ArticleC__C3B4DFAA45A2A37A");

            entity.Property(e => e.CommentId)
                .ValueGeneratedNever()
                .HasColumnName("CommentID");
            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.CommentText).HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleComments)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK__ArticleCo__Artic__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.ArticleComments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ArticleCo__UserI__52593CB8");
        });

        modelBuilder.Entity<ArticleStatus>(entity =>
        {
            entity.HasKey(e => e.ArticleStatusId).HasName("PK__ArticleS__3F0E2D6B3FEA4AEB");

            entity.ToTable("ArticleStatus");

            entity.Property(e => e.ArticleStatusId)
                .ValueGeneratedNever()
                .HasColumnName("ArticleStatusID");
            entity.Property(e => e.ArticleStatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<ArticlesDeadline>(entity =>
        {
            entity.HasKey(e => e.ArticlesDeadlineId).HasName("PK__Articles__253F2FDCDEE76182");

            entity.ToTable("ArticlesDeadline");

            entity.Property(e => e.ArticlesDeadlineId)
                .ValueGeneratedNever()
                .HasColumnName("ArticlesDeadlineID");
            entity.Property(e => e.AcademicYear).HasColumnName("academicYear");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.StartDue).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ArticlesDeadlines)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ArticlesD__UserI__44FF419A");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__Faculty__306F636EB7D89893");

            entity.ToTable("Faculty");

            entity.Property(e => e.FacultyId)
                .HasMaxLength(50)
                .HasColumnName("FacultyID");
            entity.Property(e => e.Established).HasColumnType("datetime");
            entity.Property(e => e.FacultyName).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Image__7516F4ECC58C0209");

            entity.ToTable("Image");

            entity.Property(e => e.ImageId)
                .ValueGeneratedNever()
                .HasColumnName("ImageID");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A57D64861");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<TermsAndCondition>(entity =>
        {
            entity.HasKey(e => e.TermsId).HasName("PK__TermsAnd__C05EBE0041DEAA5A");

            entity.Property(e => e.TermsId)
                .ValueGeneratedNever()
                .HasColumnName("TermsID");
            entity.Property(e => e.TermsText).HasMaxLength(500);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC5A3D4302");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FacultyId)
                .HasMaxLength(50)
                .HasColumnName("FacultyID");
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.TermsId).HasColumnName("TermsID");
            entity.Property(e => e.UserFullName).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Faculty).WithMany(p => p.Users)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK__User__FacultyID__412EB0B6");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__RoleID__403A8C7D");

            entity.HasOne(d => d.Terms).WithMany(p => p.Users)
                .HasForeignKey(d => d.TermsId)
                .HasConstraintName("FK__User__TermsID__4222D4EF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
