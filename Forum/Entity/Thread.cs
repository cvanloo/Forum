using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Entity
{
    public class Thread
    {
        [Key,Required]
        public int Id { get; set; }

        [ForeignKey("CreatorId"),Required] // Without the Fk annotation the Required anno. is ignored
        public User Creator { get; set; } 

        [Required]
        public string Title { get; set; }

        [Required]
        public string ContentPath { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool IsArchived { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<Comment> Comments { get; set; }

        [Required]
        public int ForumId { get; set; }
        [Required]
        public Forum Forum { get; set; }
    }
}
