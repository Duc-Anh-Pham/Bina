using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public string? ArticleName { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? UserId { get; set; }

    public int? ImageId { get; set; }

    public int? DocumentId { get; set; }

    public int? ArticleStatusId { get; set; }

    public Guid? ArticlesDeadlineId { get; set; }

    public string? FacultyId { get; set; }

    public virtual ICollection<ArticleComment> ArticleComments { get; set; } = new List<ArticleComment>();

    public virtual ArticleStatus? ArticleStatus { get; set; }

    public virtual ArticlesDeadline? ArticlesDeadline { get; set; }

    public virtual Document? Document { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual Image? Image { get; set; }

    public virtual User? User { get; set; }
}
