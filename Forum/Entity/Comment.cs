﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Entity
{
    public class Comment
    {
        [Key,Required]
        public int Id { get; set; }

        [ForeignKey("CreatorId"),Required]
        public User Creator { get; set; }

        [Required]
        public string Text { get; set; }

        public Comment Parent { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool IsArchived { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        
    }
}
