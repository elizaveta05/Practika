using BD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace makets.Model.Model_users;
public class UserDescription

{
    public int UdId { get; set; }

    public int UserId { get; set; }

    public string Description { get; set; } = null!;
}
