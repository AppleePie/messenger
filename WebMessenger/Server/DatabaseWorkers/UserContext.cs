using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public sealed class UserContext : DbContext
    {
        public DbSet<UserDto> Users { get; set; }
        private readonly string collectionName;
        
        public UserContext(string collectionName)
        {
            this.collectionName = collectionName;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($@"Data Source={collectionName}.db");
    }
}