using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class HelpAndSupport
{
    public int HelpSupportId { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? UserMessages { get; set; }

    public int? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
