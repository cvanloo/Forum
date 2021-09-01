using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Forum.Entity
{
    public class Comment
    {
        //private bool isDeleted;
        
        [Key,Required]
        public int Id { get; set; }

        [ForeignKey("CreatorId"),Required]
        public User Creator { get; set; }

        [Required]
        public string Text { get; set; }

        public int? ParentId { get; set; }
        public Comment Parent { get; set; }

        public ICollection<Comment> Childs { get; set; }

        [Required]
        public int ThreadId { get; set; }
        public Thread Thread { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool IsArchived { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        /*[Required]
        public bool IsDeleted
        {
            get
            {
                if (Thread.IsDeleted) return true;
                return isDeleted;
            }
            set
            {
                isDeleted = value;
            }
        }*/

        /// <summary>
        /// Recursively counts its children, including itself.
        /// </summary>
        [NotMapped]
        public int CountChildren
        {
            get
            {
                if (Childs is null) return 1;
                
                return 1 + Childs.Sum(c => c.CountChildren);
                
                //int count = 1;
                //foreach (var c in Childs)
                //{
                //    count += c.CountChildren;
                //}

                //return count;
            }
        }

        /// <summary>
        /// Recursively counts its children, including itself.
        /// </summary>
        [NotMapped]
        public int CountParents
        {
            get
            {
                if (Parent is null) return 1;

                return 1 + Parent.CountParents;
            }
        }
    }
}
