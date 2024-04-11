using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class Document
{
    public int DocumentId { get; set; }

    public string? DocumentPath { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
