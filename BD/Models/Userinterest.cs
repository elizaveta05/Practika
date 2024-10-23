using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Userinterest
{
    public int UiId { get; set; }

    public int UserId { get; set; }

    public int TagId { get; set; }

    public virtual Interest Tag { get; set; } = null!;

    public virtual Datauser User { get; set; } = null!;
}
