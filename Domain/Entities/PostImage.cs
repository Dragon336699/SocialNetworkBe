using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PostImage
    {
        [Key]
        public Guid PostId { get; set; }
        [Required]
        public required string ImageUrl { get; set; }
        public Post? post { get; set; }

    }
}
