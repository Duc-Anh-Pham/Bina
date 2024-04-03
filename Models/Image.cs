using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public string? Imagepath { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
