using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Spring.IO;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;
using Spring.Social.OAuth1;
using WebChat.Repositories.SerializableModels;
using AttributeRouting.Web.Http;
using WebChat.Model;
using WebChat.Repositories;
using System.Data.Entity;
using System.IO;


namespace WebChat.Api.Controllers
{
    [AttributeRouting.RoutePrefix("api/files")]
    public class FilesController : ApiController
    {
        private DropBox appAuth = new DropBox { Value = "avebpvpe2pr4o85", Secret = "bz5ysp3dsw0xh6l" };
        private OAuthToken userAuth = new OAuthToken("tjhu30a56q0jcnr8", "gi6rcrdf2ovvjev");
        //private IRepository<Dropbox> data;


        public FilesController()
        {
            //this.data = new DropBoxRepository(
            //    ConfigurationManager.AppSettings["MongoConnectionString"]);
        }


        private string DropboxShareFile(string path, string filename)
        {
            DropboxServiceProvider dropboxServiceProvider =
                new DropboxServiceProvider(this.appAuth.Value, this.appAuth.Secret, AccessLevel.AppFolder);
            IDropbox dropbox = dropboxServiceProvider.GetApi(userAuth.Value, userAuth.Secret);

            Entry uploadFileEntry = dropbox.UploadFileAsync(
                new FileResource(path), filename).Result;


            var sharedUrl = dropbox.GetMediaLinkAsync(uploadFileEntry.Path).Result;
            return (sharedUrl.Url); // we can download the file directly
        }


        [POST("{sessionKey}")]
        public HttpResponseMessage Post(string sessionKey)
        {

            WebChatEntities webChatContext = new WebChatEntities();

            User uploader = (from user in webChatContext.Users
                             where user.SessionKey == sessionKey
                             select user).FirstOrDefault();

            if (uploader==null)
            {
                throw new ArgumentException("Invalid sessionKey");
            }

            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];

                    string extension = GetExtension(postedFile);
                    
                    var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + uploader.Id+extension);
                    postedFile.SaveAs(filePath);

                    string avatrURL = DropboxShareFile(filePath, uploader.Id.ToString() + extension);
                    docfiles.Add(avatrURL);
                    uploader.AvatarURL = avatrURL;
                    webChatContext.SaveChanges();

                    File.Delete(filePath);
                }
                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }

        private static string GetExtension(HttpPostedFile postedFile)
        {
            int dotIndex = postedFile.FileName.LastIndexOf('.');
            string extension = postedFile.FileName.Substring(dotIndex);

            if (extension != ".jpg")
            {
                throw new ArgumentException("Only jpg and png files are suported");
            }
            return extension;
        }

        [GET("{id}")]
        public string Get(int id)
        {
            WebChatEntities webChatContext = new WebChatEntities();

            User userForImage = (from user in webChatContext.Users
                             where user.Id == id
                             select user).FirstOrDefault();

            if (userForImage == null)
            {
                throw new ArgumentException("Invalid sessionKey");
            }

            return GetUserImage(userForImage);
        }

        private string GetUserImage(User userForImage)
        {
            DropboxServiceProvider dropboxServiceProvider =
                new DropboxServiceProvider(this.appAuth.Value, this.appAuth.Secret, AccessLevel.AppFolder);
            IDropbox dropbox = dropboxServiceProvider.GetApi(userAuth.Value, userAuth.Secret);

            try
            {
                var sharedUrl = dropbox.GetMediaLinkAsync("/" + userForImage.Id.ToString() + ".jpg").Result;
                return (sharedUrl.Url);
            }
            catch (AggregateException ex)
            {
                return null;
            }
        }
    }
}