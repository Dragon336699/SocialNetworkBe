using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Group
{
    public class KickMemberRequest
    {
        [Required(ErrorMessage = "Target user ID is required")]
        public Guid TargetUserId { get; set; }
    }
}
