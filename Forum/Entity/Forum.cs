using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum.Entity
{
    [Index(nameof(Name), IsUnique = true)]
    public class Forum
    {
        [Key,Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Rules { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<Thread> Threads { get; set; }

        public ICollection<User> Members { get; set; }
        //public ICollection<Userforum> Members { get; set; }
    }
}
