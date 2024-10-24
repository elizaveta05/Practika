using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace makets.Model.Model_users;

public partial class SaveUserTagsRequest
{
    public int UiId { get; set; }

    public List<int> TagIds { get; set; }
    public int UserId { get; set; }
}
