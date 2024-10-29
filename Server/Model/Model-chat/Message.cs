using System;
using System.Collections.Generic;

namespace makets.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int ChatId { get; set; }

    public int UserSendingId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

}
