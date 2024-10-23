using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string LocationName { get; set; } = null!;

    public virtual ICollection<Datauser> Datausers { get; set; } = new List<Datauser>();
}
