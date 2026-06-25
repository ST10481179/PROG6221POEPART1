using Microsoft.EntityFrameworkCore;

namespace CyberSecurityChatbot
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserTask> Tasks { get; set; } = null!;
        public DbSet<ActivityLogEntry> Logs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
