using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Userdataregister
{
    public int UdrId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Datauser> Datausers { get; set; } = new List<Datauser>();
}
