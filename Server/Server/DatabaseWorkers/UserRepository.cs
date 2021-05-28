using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext database;
        private const string CollectionName = "users";
        public UserRepository() => database = new UserContext(CollectionName);

        public async Task<UserEntity> FindByIdAsync(Guid userId)
        {
            var result = await database.Users.FindAsync(userId);
            await database.SaveChangesAsync();
            return result;
        }

        public async Task<Guid> InsertAsync(UserEntity userEntity)
        {
            await database.AddAsync(userEntity);
            await database.SaveChangesAsync();
            return userEntity.Id;
        }

        public async Task ChangeAvatarAsync(Guid userId, string filePath)
        {
            var user = await FindByIdAsync(userId);
            user.AvatarFilePath = filePath;
            database.Update(user);
            await database.SaveChangesAsync();
        }

        public async Task<UserEntity> FindByLoginAsync(string login)
        {
            var result = await database.Users.FirstOrDefaultAsync(user => user.Login == login);
            await database.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            var user = await FindByIdAsync(userId);
            if (user is null)
                return false;
            database.Users.Remove(user);
            await database.SaveChangesAsync();
            user = await FindByIdAsync(userId);
            return true;
        }
    }
}