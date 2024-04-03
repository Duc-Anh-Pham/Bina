using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bina.Models;

public partial class Image
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ImageId { get; set; }

    public string? Imagepath { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
