using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Relationshipgoal
{
    public int GoalId { get; set; }

    public string GoalName { get; set; } = null!;

    public virtual ICollection<Usergoal> Usergoals { get; set; } = new List<Usergoal>();
}
