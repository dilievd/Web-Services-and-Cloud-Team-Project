using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using WebChat.Api.Controllers;
using WebChat.Model;
using WebChat.Repositories;
using WebChat.Repositories.SerializableModels;

namespace WebChat.Api.Resolvers
{
    public class DbWebChatResolver : IDependencyResolver
    {
        private DbContext webChatContext = new WebChatEntities();

        private IRepositoryMessages<SerializableMessage> repositoryMessages;

        private IRepositoryUsers<UserModel> repositoryUsers;// = new DbUsersRepository(webChatContext);

        private IRepositoryChats<ChatModel> repositoryChats;// = new DbChatsRepository(webChatContext);


        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(UsersController))
            {
                repositoryUsers = new DbUsersRepository(webChatContext);
                return new UsersController(repositoryUsers);
            }
            else if (serviceType == typeof(ChatsController))
            {
                repositoryChats = new DbChatsRepository(webChatContext);
                return new ChatsController(repositoryChats);
            }
            else if (serviceType == typeof(MessagesController))
            {
                repositoryMessages = new DbMessagesRepository(webChatContext);
                return new MessagesController(repositoryMessages);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose()
        {
        }
    }
}