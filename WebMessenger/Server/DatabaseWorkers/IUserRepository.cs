using System;
using System.Threading.Tasks;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public interface IUserRepository
    {
        public Task<Guid> InsertAsync(UserDto user);
        public Task<UserDto> FindByIdAsync(Guid userId);
        public Task<UserDto> FindByLoginAsync(string login);
        public Task ChangeAvatarAsync(Guid userId, string filename);
        public Task<bool> DeleteAsync(Guid userId);
    }
}