using Domain.Enum.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserRelation
    {
        public Guid UserId { get; set; }
        public Guid TargetUserId { get; set; }
        public UserRelationType RelationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User? User { get; set; }
        public User? TargetUser { get; set; }
    }
}
