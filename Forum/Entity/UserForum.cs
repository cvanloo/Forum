using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Entity
{
    public class UserForum
    {
        [Key,Required]
        public int Id { get; set; }

        [ForeignKey("UserId"),Required]
        public User UserModel { get; set; }

        [ForeignKey("ForumId"),Required]
        public Forum ForumModel { get; set; }

        [Required]
        public DateTime Joined { get; set; }

        [Required]
        public bool IsBlocked { get; set; }

        [Required]
        public int ModLevel { get; set; }
    }
}
