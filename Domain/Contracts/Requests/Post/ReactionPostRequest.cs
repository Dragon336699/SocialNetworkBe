using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Requests.Post
{
    public class ReactionPostRequest
    {
        [Required]
        public Guid PostId { get; set; }

        [Required]
        public required string Reaction { get; set; } 
    }
}
