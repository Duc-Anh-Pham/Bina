using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class CommentFeedback
{
    public Guid CommentFeedbackId { get; set; }

    public int? UserId { get; set; }

    public int? ArticleId { get; set; }

    public DateTime? CommentDay { get; set; }

    public string? ContentFeedback { get; set; }

    public virtual Article? Article { get; set; }

    public virtual User? User { get; set; }
}
