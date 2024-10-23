using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Match
{
    public int MatchId { get; set; }

    public int User1Id { get; set; }

    public int User2Id { get; set; }

    public DateOnly Timestamp { get; set; }

    public virtual Datauser User1 { get; set; } = null!;

    public virtual Datauser User2 { get; set; } = null!;
}
