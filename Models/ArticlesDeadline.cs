using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class ArticlesDeadline
{
    public Guid ArticlesDeadlineId { get; set; }

    public int? UserId { get; set; }

    public DateTime? StartDue { get; set; }

    public DateTime? DueDate { get; set; }

    public int? AcademicYear { get; set; }

    public string? TermName { get; set; }

    public string? TermTitle { get; set; }

    public string? FacultyId { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual Faculty? Faculty { get; set; }

    public virtual User? User { get; set; }
}
