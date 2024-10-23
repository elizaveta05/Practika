using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Userlike
{
    public int UlId { get; set; }

    public int UserId { get; set; }

    public int LikedUserId { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Datauser LikedUser { get; set; } = null!;

    public virtual Datauser User { get; set; } = null!;
}
