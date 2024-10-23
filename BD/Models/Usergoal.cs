using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Usergoal
{
    public int UgId { get; set; }

    public int UserId { get; set; }

    public int GoalId { get; set; }

    public virtual Relationshipgoal Goal { get; set; } = null!;

    public virtual Datauser User { get; set; } = null!;
}
