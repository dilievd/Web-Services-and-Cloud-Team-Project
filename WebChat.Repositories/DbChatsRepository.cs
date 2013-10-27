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
    public class DbChatsRepository : IRepositoryChats<ChatModel>
    {
        private DbContext dbContext;
        private DbSet<User> userSet;
        private DbSet<Chat> chatSet;
        private DbSet<Message> messageSet;

        public DbChatsRepository(DbContext dbContext)
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





        public ChatModel New(int id, string sessionKey)
        {
            User userById = (from user in this.userSet
                             where id == user.Id
                             select user).FirstOrDefault();

            if (userById==null)
            {
                throw new ArgumentException(String.Format("User with id {0} does not exist", id));
            }

            User userByKey = (from user in this.userSet
                              where sessionKey == user.SessionKey
                             select user).FirstOrDefault();

            if (userByKey == null)
            {
                throw new ArgumentException("Invalid sessionKey");
            }

            if (this.chatSet.Any(c => (c.User1.Id==userById.Id && c.User2.Id==userByKey.Id) || (c.User2.Id==userById.Id && c.User1.Id==userByKey.Id)))
            {
                throw new ArgumentException("Chat already exists");
            }
            //TODO: Get channel
            Chat newChat = new Chat();
            if (userById.Id<userByKey.Id)
            {
                newChat.User1 = userById;
                newChat.User2 = userByKey;
            }
            else if (userById.Id > userByKey.Id)
            {
                newChat.User2 = userById;
                newChat.User1 = userByKey;
            }
            else
            {
                throw new ArgumentException("Cannot chat with yourself");
            }


            this.chatSet.Add(newChat);
            this.dbContext.SaveChanges();
            newChat.Channel = newChat.Id.ToString() +"-"+ newChat.User1.Id.ToString()+"-" + newChat.User2.Id.ToString();
            this.dbContext.SaveChanges();

            return new ChatModel()
            {
                Id = newChat.Id,
                Channel = newChat.Channel,
                User1 = new UserModel() { Id = newChat.User1.Id, Name = newChat.User1.Name },
                User2 = new UserModel() { Id = newChat.User2.Id, Name = newChat.User2.Name }
            };
        }

        public ChatModel Get(int id, string sessionKey)
        {
            User userById = (from user in this.userSet
                             where id == user.Id
                             select user).FirstOrDefault();

            if (userById == null)
            {
                throw new ArgumentException(String.Format("User with id {0} does not exist", id));
            }

            User userByKey = (from user in this.userSet
                              where sessionKey == user.SessionKey
                              select user).FirstOrDefault();

            if (userByKey == null)
            {
                throw new ArgumentException("Invalid sessionKey");
            }

            Chat result = (from chat in this.chatSet.Include("User1").Include("User2")
                           where (chat.User1.Id == userById.Id && chat.User2.Id == userByKey.Id)
                           || (chat.User1.Id == userByKey.Id && chat.User2.Id == userById.Id)
                           select chat).FirstOrDefault();

            if (result==null)
            {
                throw new ArgumentException("No chat started");
            }

            return new ChatModel()
            {
                Id = result.Id,
                Channel = result.Channel,
                User1 = new UserModel() { Id = result.User1.Id, Name = result.User1.Name },
                User2 = new UserModel() { Id = result.User2.Id, Name = result.User2.Name }
            };
            
        }

        public IQueryable<ChatModel> All(string sessionKey)
        {
            User userByKey = (from user in this.userSet
                              where sessionKey == user.SessionKey
                              select user).FirstOrDefault();

            if (userByKey == null)
            {
                throw new ArgumentException("Invalid sessionKey");
            }

            var result = from chat in this.chatSet.Include("User1").Include("User2")
                         where chat.User1.SessionKey==sessionKey || chat.User2.SessionKey==sessionKey
                         select new ChatModel()
                         {
                             Id = chat.Id,
                             Channel = chat.Channel,
                             User1 = new UserModel() { Id = chat.User1.Id, Name = chat.User1.Name },
                             User2 = new UserModel() { Id = chat.User2.Id, Name = chat.User2.Name }
                         };

            return result;
        }


        public void SendMessage(int id, string sessionKey, string content)
        {

            User userByKey = (from user in this.userSet
                              where sessionKey == user.SessionKey
                              select user).FirstOrDefault();

            if (userByKey == null)
            {
                throw new ArgumentException("Invalid sessionKey");
            }

            ChatModel currentChat = this.Get(id, sessionKey);

            if (currentChat==null)
            {
                throw new ArgumentException("Chat does not exist. Please start a chat with this user");
            }

            this.messageSet.Add(new Message() { ChatID=currentChat.Id, MessageContent=content, OwnerId=userByKey.Id});
            this.dbContext.SaveChanges();
        }
    }
}
