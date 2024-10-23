using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Userdescription
{
    public int UdId { get; set; }

    public int UserId { get; set; }

    public string Description { get; set; } = null!;

    public virtual Datauser User { get; set; } = null!;
}
