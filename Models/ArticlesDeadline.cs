namespace Bina.Models;

public partial class ArticlesDeadline
{
    public int ArticlesDeadlineId { get; set; }

    public int? UserId { get; set; }

    public DateOnly? StartDue { get; set; }

    public DateOnly? DueDate { get; set; }

    public int? AcademicYear { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual User? User { get; set; }
}
