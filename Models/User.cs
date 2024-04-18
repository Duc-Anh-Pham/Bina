namespace Bina.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? PhoneNumber { get; set; }

    public DateOnly? DoB { get; set; }

    public DateTime? DateCreated { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public string? FacultyId { get; set; }

    public byte? Status { get; set; }

    public int? TermsId { get; set; }

    public virtual ICollection<ArticleComment> ArticleComments { get; set; } = new List<ArticleComment>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<ArticlesDeadline> ArticlesDeadlines { get; set; } = new List<ArticlesDeadline>();

    public virtual Faculty? Faculty { get; set; }

    public virtual Role? Role { get; set; }

    public virtual TermsAndCondition? Terms { get; set; }
    public bool RememberMe { get; set; }
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }
}
