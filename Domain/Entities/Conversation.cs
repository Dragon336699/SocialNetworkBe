using Domain.Enum.Conversation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public ConversationType Type {  get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
