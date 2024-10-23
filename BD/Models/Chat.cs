using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Chat
{
    public int ChatId { get; set; }

    public int User1Id { get; set; }

    public int User2Id { get; set; }

    public DateTime TimeCreated { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Datauser User1 { get; set; } = null!;

    public virtual Datauser User2 { get; set; } = null!;
}
