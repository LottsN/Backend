using Microsoft.EntityFrameworkCore;

namespace lab2.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<BlacklistToken> BlacklistedTokens { get; set; }
        public DbSet<TaskData> Tasks { get; set; }
        public DbSet<Solution> Solutions { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Solution>()
                .HasOne<TaskData>()
                .WithMany()
                .HasForeignKey(solution => solution.taskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Solution>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(solution => solution.authorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<User>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(user => user.roleId);


            /*
            modelBuilder.Entity<TaskData>()
                .HasOne<Topic>()
                .WithMany()
                .HasForeignKey(task => task.topicId);
            */
        }
    }
}
