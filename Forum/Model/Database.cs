using Microsoft.EntityFrameworkCore;
using Forum.Entity;

namespace Forum.Model
{
    /// <summary>
    /// Specifies the database context
    /// </summary>
    public class Database : DbContext
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<User> Users { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Entity.Forum> Forums { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Thread> Threads { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Comment> Comments { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Session> Sessions { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<PwReset> PwResets { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Tag> Tags { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Setting> Settings { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DbSet<Chat> Chats { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Configuration options</param>
        public Database(DbContextOptions<Database> options) : base(options) { } 

        /// <summary>
        /// Configures the database tables.
        /// </summary>
        /// <param name="modelBuilder">Model builder</param>
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
            //modelBuilder.Entity<UserForum>()
            //    .Property(uf => uf.Joined)
            //    .HasDefaultValueSql("NOW()");

            //modelBuilder.Entity<UserForum>()
            //    .Property(uf => uf.IsBlocked)
            //    .HasDefaultValue(false);

            //modelBuilder.Entity<UserForum>()
            //    .Property(uf => uf.ModLevel)
            //    .HasDefaultValue(0);

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

            // PwReset
            modelBuilder.Entity<PwReset>()
                .Property(p => p.Timestamp)
                .HasDefaultValueSql("NOW()");


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
				.HasIndex(s => new { s.SettingKey, s.UserId })
				.IsUnique();

            // Tag
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Session
            //modelBuilder.Entity<Session>()
            //    .HasKey(s => new { s.UserId, s.Identifier });

            //modelBuilder.Entity<Session>()
            //    .HasIndex(s => new { s.User, s.Identifier })
            //    .IsUnique();

            /* Mapping multiple relations between the same tables.
             * Multiple relations between two tables need to be
             * mapped manually.
             */
            // One-to-Many
            modelBuilder.Entity<User>()
                .HasMany(u => u.Threads)
                .WithOne(t => t.Creator)
                .HasForeignKey(t => t.CreatorId);

            // Many-to-Many
            modelBuilder.Entity<User>()
                .HasMany(u => u.SavedThreads)
                .WithMany(t => t.Saviors);
        }

        /* NOTE1: In order to make properties from Entities readonly,
         * use a private setter and a constructor
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
         * NOTE4: (one-to-many): In the ORM the fk is displayed in 'Forum', in the db it is in 'Thread'.
         * In order to make the fk in 'Thread' non-nullable it has to be specified in the
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
         *
         * NOTE5: Multiple relationships between the same two tables require to be manually mapped.
         * Relationship 1; One-to-Many
         * modelBuilder.Entity<User>()
         *     .HasMany(u => u.Threads)
         *     .WithOne(t => t.Creator)
         *     .HasForeignKey(t => t.CreatorId);
         *
         * Relationship 2; Many-to-Many
         * modelBuilder.Entity<User>()
         *     .HasMany(u => u.SavedThreads)
         *     .WithMany(t => t.Saviors);
         */
    }
}
