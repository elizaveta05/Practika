using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Interest
{
    public int InterestId { get; set; }

    public string InterestName { get; set; } = null!;

    public virtual ICollection<Userinterest> Userinterests { get; set; } = new List<Userinterest>();
}
