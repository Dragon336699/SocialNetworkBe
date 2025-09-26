using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SearchingHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string? Content { get; set; }
        [AllowNull]
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        [AllowNull]
        public Guid? GroupId { get; set; }
        public Group? Group { get; set; }
    }
}
