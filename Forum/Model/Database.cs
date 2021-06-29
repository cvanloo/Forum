using System;
using Microsoft.EntityFrameworkCore;
using Forum.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Forum.Model
{
    /// <summary>
    /// Specifies the database context
    /// </summary>
    public class Database : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Entity.Forum> Forums { get; set; }
        public DbSet<UserForum> UserForums { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public Database(DbContextOptions<Database> options) : base(options) { } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* Set default values */

            // User
            modelBuilder.Entity<User>()
                .Property(u => u.Created)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<User>()
                .Property(u => u.IsBlocked)
                .HasDefaultValue(false);

            modelBuilder.Entity<User>()
                .Property(u => u.IsDeleted)
                .HasDefaultValue(false);

            //modelBuilder.Entity<User>()
            //    .Property(u => u.Follows)
            //    .HasColumnName("FollowerId");

            //modelBuilder.Entity<User>()
            //    .Property(u => u.Followed)
            //    .HasColumnName("FolloweeId");

            // Forum
            modelBuilder.Entity<Entity.Forum>()
                .Property(f => f.Created)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Entity.Forum>()
                .Property(f => f.IsDeleted)
                .HasDefaultValue(false);

            // Thread
            modelBuilder.Entity<Thread>()
                .Property(t => t.Created)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Thread>()
                .Property(t => t.IsArchived)
                .HasDefaultValue(false);

            modelBuilder.Entity<Thread>()
                .Property(t => t.IsDeleted)
                .HasDefaultValue(false);

            // Comment
            modelBuilder.Entity<Comment>()
                .Property(c => c.Created)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Comment>()
                .Property(c => c.IsArchived)
                .HasDefaultValue(false);

            modelBuilder.Entity<Comment>()
                .Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            // UserForum
            modelBuilder.Entity<UserForum>()
                .Property(uf => uf.Joined)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<UserForum>()
                .Property(uf => uf.IsBlocked)
                .HasDefaultValue(false);

            modelBuilder.Entity<UserForum>()
                .Property(uf => uf.ModLevel)
                .HasDefaultValue(0);

            // Chat
            modelBuilder.Entity<Chat>()
                .Property(c => c.Created)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Chat>()
                .Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            // ChatMessage
            modelBuilder.Entity<ChatMessage>()
                .Property(cm => cm.Sent)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<ChatMessage>()
                .Property(cm => cm.IsDeleted)
                .HasDefaultValue(false);


            /* Set unique constraints */

            // These are now created using annotations
            //// User
            //modelBuilder.Entity<User>()
            //    .HasIndex(u => u.AccountName)
            //    .IsUnique();

            //modelBuilder.Entity<User>()
            //    .HasIndex(u => u.Email)
            //    .IsUnique();

            //// Forum
            //modelBuilder.Entity<Entities.Forum>()
            //    .HasIndex(f => f.Name)
            //    .IsUnique();

            // Setting
            modelBuilder.Entity<Setting>()
				.HasIndex(s => new { s.Key, s.UserId })
				.IsUnique();

            // Session
            //modelBuilder.Entity<Session>()
            //    .HasKey(s => new { s.UserId, s.Identifier });

            //modelBuilder.Entity<Session>()
            //    .HasIndex(s => new { s.User, s.Identifier })
            //    .IsUnique();
        }

        /* NOTE: In order to make properties from Entities readonly,
         * use a private setter and a contstructor
         * 
         * public User Creator { get; private set; }
         * 
         * public Comment(User creator)
         * {
         *      Creator = creator;
         * }
         * 
         * NOTE2: Use the [NotMapped] annotation to indicate that a property
         * should not be mapped to the database.
         * 
         * NOTE3: The [Required] annotation is ignored, except when the foreign key is explicitly
         * marked as one:
         * 
         * [ForeignKey("CreatorId"),Required] // Without the Fk annotation the Required anno. is ignored
         * public User Creator { get; set; }
         * 
         * NOTE4 (one-to-many): In the ORM the fk is displayed in 'Forum', in the db it is in 'Thread'.
         * In order to make the fk in 'Thread' non-nullable it has to be spicified in the
         * 'Thread' class of the orm too, as an int.
         * 
         * public class Forum {
         *     public ICollection<Thread> Threads { get; set; }
         * }
         * 
         * public class Thread {
         *     [Required]
         *     public int ForumId { get; set; }
         * }
         */
    }
}
