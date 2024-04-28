using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class ArticleLike
{
    public Guid ArticleLikeId { get; set; }

    public int? UserId { get; set; }

    public int? ArticleId { get; set; }

    public virtual Article? Article { get; set; }

    public virtual User? User { get; set; }
}
