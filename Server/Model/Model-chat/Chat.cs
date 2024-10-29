using System;
using System.Collections.Generic;

namespace makets.Models;

public partial class Chat
{
    public int ChatId { get; set; }

    public int User1Id { get; set; }

    public int User2Id { get; set; }

    public DateTime TimeCreated { get; set; }
}
