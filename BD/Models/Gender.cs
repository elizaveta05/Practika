using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Gender
{
    public int GenderId { get; set; }

    public string GenderName { get; set; } = null!;

    public virtual ICollection<Datauser> Datausers { get; set; } = new List<Datauser>();
}
