using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Conversation
{
    public class ChangeNicknameRequest
    {
        [Required]
        public Guid ConversationId { get; set; }

        [Required]
        public Guid TargetUserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Nickname must be between 1 and 50 characters")]
        public required string NewNickname { get; set; }
    }
}
