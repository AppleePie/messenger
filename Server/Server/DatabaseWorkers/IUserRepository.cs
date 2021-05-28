using System;
using System.Threading.Tasks;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public interface IUserRepository
    {
        public Task<Guid> InsertAsync(UserEntity user);
        public Task<UserEntity> FindByIdAsync(Guid userId);
        public Task<UserEntity> FindByLoginAsync(string login);
        public Task ChangeAvatarAsync(Guid userId, string filename);
        public Task<bool> DeleteAsync(Guid userId);
    }
}