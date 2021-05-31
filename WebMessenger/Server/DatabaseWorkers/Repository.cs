using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.DatabaseWorkers
{
    public class Repository
    {
        private readonly DatabaseContext database;

        public Repository(DatabaseContext database)
        {
            this.database = database;
            FillRelations(database);
        }

        private static void FillRelations(DatabaseContext database)
        {
            foreach (var userToChat in database.UserToChats)
            {
                userToChat.Chat = database.Find<Chat>(userToChat.ChatId);
                userToChat.User = database.Find<User>(userToChat.UserId);
            }
            
            foreach (var userToChat in database.UserToMessages)
            {
                userToChat.Message = database.Find<Message>(userToChat.MessageId);
                userToChat.User = database.Find<User>(userToChat.UserId);
            }
            
            foreach (var chatToMessage in database.ChatToMessages)
            {
                chatToMessage.Chat = database.Find<Chat>(chatToMessage.ChatId);
                chatToMessage.Message = database.Find<Message>(chatToMessage.MessageId);
            }

            database.SaveChanges();
        }

        public IEnumerable<User> GetUsers() => database.Users;

        public async Task<TEntity> FindByIdAsync<TEntity>(Guid id) where TEntity : class
        {
            var result = await database.FindAsync<TEntity>(id);
            await database.SaveChangesAsync();
            return result;
        }

        public async Task<TEntity> InsertAsync<TEntity>(TEntity userDto) where TEntity : class
        {
            var insertedUser = (await database.AddAsync(userDto)).Entity;
            await database.SaveChangesAsync();
            return insertedUser;
        }

        public async Task<bool> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            database.Update(entity);
            await database.SaveChangesAsync();
            return true;
        }

        public async Task<User> FindByLoginAsync(string login)
        {
            var result = await database.Users.FirstOrDefaultAsync(user => user.Login == login);
            await database.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteAsync<TEntity>(Guid userId) where TEntity : class
        {
            var user = await FindByIdAsync<TEntity>(userId);
            if (user is null)
                return false;
            database.Remove(user);
            await database.SaveChangesAsync();
            return true;
        }
    }
}