using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebChat.Model;
using WebChat.Repositories.SerializableModels;

namespace WebChat.Repositories
{
    public class DbMessagesRepository :IRepositoryMessages<SerializableMessage>
    {
        private DbContext dbContext;
        private DbSet<User> userSet;
        private DbSet<Chat> chatSet;
        private DbSet<Message> messageSet;


        public DbMessagesRepository(DbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentException("An instance of DbContext is required to use this repository.", "context");
            }

            this.dbContext = dbContext;
            this.userSet = this.dbContext.Set<User>();
            this.chatSet = this.dbContext.Set<Chat>();
            this.messageSet = this.dbContext.Set<Message>();
        }

        public IEnumerable<SerializableMessage> Get(int chatId, string sessionKey)
        {
            User userByKey = (from user in this.userSet
                              where sessionKey == user.SessionKey
                              select user).FirstOrDefault();

            if (userByKey == null)
            {
                throw new ArgumentException("Invalid sessionKey");
            }

            var chatById = (from chat in this.chatSet
                            where chatId == chat.Id
                            select chat).FirstOrDefault();

            if (chatById == null)
            {
                throw new ArgumentException("Invalid chat id");
            }

            var messages = (from m in this.messageSet
                           where m.ChatID == chatById.Id
                           select new SerializableMessage 
                           { Content = m.MessageContent,
                               OwnerId = m.OwnerId, 
                               OwnerName = m.User.Name })
                               .ToList();

            return messages;
        }
    }
}
