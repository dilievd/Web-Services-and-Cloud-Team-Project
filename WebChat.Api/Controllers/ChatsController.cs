using AttributeRouting;
using AttributeRouting.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebChat.Repositories;
using WebChat.Repositories.SerializableModels;

namespace WebChat.Api.Controllers
{
    [AttributeRouting.RoutePrefix("api/chats")]
    public class ChatsController : ApiController
    {
        private IRepositoryChats<ChatModel> chatRepository;

        public ChatsController(IRepositoryChats<ChatModel> chatsRepository)
        {
            this.chatRepository = chatsRepository;
        }

        [GET("{sessionkey}")]
        public IEnumerable<ChatModel> Get(string sessionkey)
        {
            return this.chatRepository.All(sessionkey).ToList();
        }


        [GET("new/{id}/{sessionkey}")]
        public ChatModel Get(int id, string sessionKey)
        {
            return this.chatRepository.New(id, sessionKey);

        }

        [POST("senMessage/{id}/{sessionkey}")]
        public void Post([FromBody]SerializableMessage value, int id, string sessionkey)
        {
            this.chatRepository.SendMessage(id, sessionkey, value.Content);
        }
    }
}
