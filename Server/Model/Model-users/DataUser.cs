using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace makets.Model.Model_users;

public partial class DataUser()
{
    public int UserId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public int GenderId { get; set; }

    public int LocationId { get; set; }

    public int UdrId { get; set; }
}