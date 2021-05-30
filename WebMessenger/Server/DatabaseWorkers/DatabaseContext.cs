using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public sealed class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<UserToChat> UserToChats { get; set; }
        private const string CollectionName = "users";
        
        public DatabaseContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($@"Data Source={CollectionName}.db");
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserToChat>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserToChats)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserToChat>()
                .HasOne(x => x.Chat)
                .WithMany(x => x.UserToChats)
                .HasForeignKey(x => x.ChatId);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}