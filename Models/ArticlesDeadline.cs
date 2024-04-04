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

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual User? User { get; set; }
}
