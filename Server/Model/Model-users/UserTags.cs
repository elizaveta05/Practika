using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace makets.Model.Model_users;

public partial class UserTags
{
    [Required]
    public int UiId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int TagId { get; set; }
}
