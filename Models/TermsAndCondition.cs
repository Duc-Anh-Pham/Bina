using System;
using System.Collections.Generic;

namespace Bina.Models;

public partial class TermsAndCondition
{
    public int TermsId { get; set; }

    public string? TermsText { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
