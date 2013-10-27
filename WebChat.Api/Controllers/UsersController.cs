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
    [AttributeRouting.RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private IRepositoryUsers<UserModel> userRepository;

        public UsersController(IRepositoryUsers<UserModel> usersRepository)
        {
            this.userRepository = usersRepository;
        }

        // GET api/values
        public IEnumerable<UserModel> Get()
        {
            return this.userRepository.All().ToList();
        }

        // GET api/values/5
        public UserModel Get(int id)
        {
            return this.userRepository.Get(id);
        }

        [GET("register/{username}/{password}")]
        public UserModel GetRegisterUser(string username, string password)
        {
            return this.userRepository.Add(new UserModel() { Name = username, PassWord = password });
        }

        [GET("login/{username}/{password}")]
        public UserModel Get(string username, string password)
        {

            return this.userRepository.Get(username, password);
        }

        [GET("logout/{sessionkey}")]
        public void Get(string sessionkey)
        {

            this.userRepository.Logout(sessionkey);
        }



        //// POST api/values
        //public HttpResponseMessage Post([FromBody]UserModel value)
        //{
        //    var addedUser = this.userRepository.Add(value);

        //    var response = Request.CreateResponse<UserModel>(HttpStatusCode.OK, addedUser);
        //    var resourceLink = Url.Link("DefaultApi", new { id = addedUser.Id });

        //    response.Headers.Location = new Uri(resourceLink);
        //    return response;
        //}

        // PUT api/values/5
        //public HttpResponseMessage Put(int id, [FromBody]UserModel value)
        //{
        //    var addedUser = this.userRepository.Update(id, value);

        //    var response = Request.CreateResponse<UserModel>(HttpStatusCode.OK, addedUser);
        //    var resourceLink = Url.Link("DefaultApi", new { id = addedUser.Id });

        //    response.Headers.Location = new Uri(resourceLink);
        //    return response;
        //}

        // DELETE api/values/5
        //public HttpResponseMessage Delete(int id)
        //{
        //    this.userRepository.Delete(id);

        //    var response = Request.CreateResponse(HttpStatusCode.OK);

        //    return response;
        //}
    }
}