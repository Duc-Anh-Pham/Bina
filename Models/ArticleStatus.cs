using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class ArticleStatus
{
    public int ArticleStatusId { get; set; }

    public string? ArticleStatusName { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
