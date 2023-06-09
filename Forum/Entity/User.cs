using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum.Entity
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(AccountName), IsUnique = true)]
    public class User
    {
		[Key, Required]
		public int Id { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
        public string AccountName { get; set; }

        public string DisplayName { get; set; }

        [Required]
        public string PwHash { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool IsBlocked { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public ICollection<User> Followees { get; set; } // Our "followees", the users we follow

        [Required]
        public ICollection<User> Followers { get; set; } // Our followers, the users that follow us

        public ICollection<Setting> Settings { get; set; }

        public ICollection<Chat> Chats { get; set; }

        public ICollection<Forum> Forums { get; set; }
        //public ICollection<UserForum> Forums { get; set; }

        // Threads created by this user
        public ICollection<Thread> Threads { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ICollection<Session> Sessions { get; set; }

        public ICollection<PwReset> PwResets { get; set; }

        // Threads saved by this user
        public ICollection<Thread> SavedThreads { get; set; }
    }
}