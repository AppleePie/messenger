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

        public async Task<UserDto> FindByIdAsync(Guid userId)
        {
            var result = await database.Users.FindAsync(userId);
            await database.SaveChangesAsync();
            return result;
        }

        public async Task<Guid> InsertAsync(UserDto userDto)
        {
            await database.AddAsync(userDto);
            await database.SaveChangesAsync();
            return userDto.Id;
        }

        public async Task ChangeAvatarAsync(Guid userId)
        {
            var user = await FindByIdAsync(userId);
            user.AvatarFilePath = userId.ToString();
            database.Update(user);
            await database.SaveChangesAsync();
        }

        public async Task<UserDto> FindByLoginAsync(string login)
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
            return true;
        }
    }
}