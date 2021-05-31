using System;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public sealed class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserToChat> UserToChats { get; set; }
        public DbSet<UserToMessage> UserToMessages { get; set; }
        public DbSet<ChatToMessage> ChatToMessages { get; set; }
        private const string CollectionName = "users";
        
        public DatabaseContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($@"Data Source={CollectionName}.db");
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserToChat>().ToTable(nameof(UserToChats));
            modelBuilder.Entity<UserToMessage>().ToTable(nameof(UserToMessages));
            modelBuilder.Entity<ChatToMessage>().ToTable(nameof(ChatToMessages));
            
            modelBuilder.Entity<UserToMessage>()
                .HasOne(m => m.User)
                .WithMany(user => user.UserToMessages);
            
            modelBuilder.Entity<UserToMessage>()
                .HasOne(m => m.Message)
                .WithOne(message => message.UserToMessage);
            
            modelBuilder.Entity<ChatToMessage>()
                .HasOne(x => x.Chat)
                .WithMany(x => x.ChatToMessages)
                .HasForeignKey(x => x.ChatId);

            modelBuilder.Entity<ChatToMessage>()
                .HasOne(x => x.Message)
                .WithOne(x => x.ChatToMessage);
            
            modelBuilder.Entity<UserToChat>()
                .HasOne(x => x.Chat)
                .WithMany(x => x.UserToChats)
                .HasForeignKey(x => x.ChatId);
            
            modelBuilder.Entity<UserToChat>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserToChats)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserToChat>()
                .HasOne(x => x.Chat)
                .WithMany(x => x.UserToChats)
                .HasForeignKey(x => x.ChatId);
        }
    }
}