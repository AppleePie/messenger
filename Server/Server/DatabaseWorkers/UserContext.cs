using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public sealed class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        // public UserContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Filename=users.db");
    }
}