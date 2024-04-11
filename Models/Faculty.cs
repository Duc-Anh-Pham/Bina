using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class Faculty
{
    public string FacultyId { get; set; } = null!;

    public string? FacultyName { get; set; }

    public DateTime? Established { get; set; }

    public byte? Status { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<ArticlesDeadline> ArticlesDeadlines { get; set; } = new List<ArticlesDeadline>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
