using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Userphotopprofile
{
    public int UppId { get; set; }

    public int? UserId { get; set; }

    public byte[]? Photo { get; set; }

    public virtual Datauser? User { get; set; }
}
