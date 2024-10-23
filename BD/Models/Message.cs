using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int ChatId { get; set; }

    public int UserSendingId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual Datauser UserSending { get; set; } = null!;
}
