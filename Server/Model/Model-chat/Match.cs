using System;
using System.Collections.Generic;

namespace makets.Models;

public partial class Match
{
    public int MatchId { get; set; }

    public int User1Id { get; set; }

    public int User2Id { get; set; }

    public DateOnly Timestamp { get; set; }
}
