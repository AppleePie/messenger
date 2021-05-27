using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public static class UserRepository
    {
        private static readonly UserContext Database = new();

        public static async Task<User> FindByIdAsync(Guid userId)
        {
            var result = await Database.Users.FindAsync(userId);
            await Database.SaveChangesAsync();
            return result;
        }

        public static async Task<Guid> InsertAsync(User user)
        {
            await Database.AddAsync(user);
            await Database.SaveChangesAsync();
            return user.Id;
        }

        public static async Task ChangeUserAvatarAsync(Guid userId, string filePath)
        {
            var user = await FindByIdAsync(userId);
            user.AvatarFilePath = filePath;
            Database.Update(user);
            await Database.SaveChangesAsync();
        }

        public static async Task<User> FindByNameAsync(string userName)
        {
            var result = await Database.Users.FirstOrDefaultAsync(user => user.Name == userName);
            await Database.SaveChangesAsync();
            return result;
        }
    }
}