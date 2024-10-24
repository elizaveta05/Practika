using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace makets.Model.Model_users;

public partial class UserGoals
{
    [Required]
    public int UgId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int GoalId { get; set; }
}
