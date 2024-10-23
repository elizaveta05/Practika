using System;
using System.Collections.Generic;

namespace BD.Models;

public partial class Datauser
{
    public int UserId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public int GenderId { get; set; }

    public int LocationId { get; set; }

    public int UdrId { get; set; }

    public virtual ICollection<Chat> ChatUser1s { get; set; } = new List<Chat>();

    public virtual ICollection<Chat> ChatUser2s { get; set; } = new List<Chat>();

    public virtual Gender Gender { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<Match> MatchUser1s { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchUser2s { get; set; } = new List<Match>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Userdataregister Udr { get; set; } = null!;

    public virtual ICollection<Userdescription> Userdescriptions { get; set; } = new List<Userdescription>();

    public virtual ICollection<Usergoal> Usergoals { get; set; } = new List<Usergoal>();

    public virtual ICollection<Userinterest> Userinterests { get; set; } = new List<Userinterest>();

    public virtual ICollection<Userlike> UserlikeLikedUsers { get; set; } = new List<Userlike>();

    public virtual ICollection<Userlike> UserlikeUsers { get; set; } = new List<Userlike>();
}
