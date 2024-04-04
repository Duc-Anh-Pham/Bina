using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class ArticleComment
{
    public int CommentId { get; set; }

    public int? UserId { get; set; }

    public int? ArticleId { get; set; }

    public DateOnly? CommentDay { get; set; }

    public string? CommentText { get; set; }

    public virtual Article? Article { get; set; }

    public virtual User? User { get; set; }
}
