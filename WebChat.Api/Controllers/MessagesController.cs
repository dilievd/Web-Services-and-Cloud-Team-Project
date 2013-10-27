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
    [AttributeRouting.RoutePrefix("api/messages")]
    public class MessagesController : ApiController
    {
        private IRepositoryMessages<SerializableMessage> messageRepository;

        public MessagesController(IRepositoryMessages<SerializableMessage> messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        // GET api/messages
        [GET("{chatId}/{sessionkey}")]
        public IEnumerable<SerializableMessage> Get(int chatId, string sessionkey)
        {
            return this.messageRepository.Get(chatId, sessionkey);
        }

        // GET api/messages/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/messages
        public void Post([FromBody]string value)
        {
        }

        // PUT api/messages/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/messages/5
        public void Delete(int id)
        {
        }
    }
}
