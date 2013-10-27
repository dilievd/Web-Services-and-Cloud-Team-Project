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
    public class DbUsersRepository : IRepositoryUsers<UserModel>
    {
        private const string SessionKeyChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const int SessionKeyLen = 50;
        protected static Random rand = new Random();

        private DbContext dbContext;
        private DbSet<User> userSet;

        public DbUsersRepository(DbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentException("An instance of DbContext is required to use this repository.", "context");
            }

            this.dbContext = dbContext;
            this.userSet = this.dbContext.Set<User>();
        }

        private static string GenerateSessionKey(int userId)
        {
            StringBuilder keyChars = new StringBuilder(50);
            keyChars.Append(userId.ToString());
            while (keyChars.Length < SessionKeyLen)
            {
                int randomCharNum;
                lock (rand)
                {
                    randomCharNum = rand.Next(SessionKeyChars.Length);
                }
                char randomKeyChar = SessionKeyChars[randomCharNum];
                keyChars.Append(randomKeyChar);
            }

            string sessionKey = keyChars.ToString();
            return sessionKey;
        }

        public UserModel Add(UserModel entity)
        {
            User newUser = new User() { Name = entity.Name, Password = entity.PassWord };
            if (this.userSet.Any(u => u.Name.ToLower() == entity.Name.ToLower()))
            {
                throw new ArgumentException("A user with this name already exists");
            }

            this.userSet.Add(newUser);

            this.dbContext.SaveChanges();
            string sessionKey = GenerateSessionKey(newUser.Id);
            newUser.SessionKey = sessionKey;
            this.dbContext.SaveChanges();

            return new UserModel() { Id = newUser.Id, Name = newUser.Name, SessionKey = newUser.SessionKey };
        }

        public UserModel Update(int id, UserModel entity)
        {
            User userToEdit = this.userSet.FirstOrDefault(u => u.Id == id);
            if (userToEdit == null)
            {
                throw new ArgumentException(String.Format("User with id {0} does not exist", id));
            }

            if (this.userSet.Any(u => u.Name.ToLower() == entity.Name.ToLower()))
            {
                throw new ArgumentException("This username is already taken");
            }

            userToEdit.Name = entity.Name;
            this.dbContext.SaveChanges();

            return new UserModel() { Id = userToEdit.Id, Name = userToEdit.Name };
        }

        public void Delete(int id)
        {
            User userToDelete = this.userSet.FirstOrDefault(u => u.Id == id);
            if (userToDelete == null)
            {
                throw new ArgumentException(String.Format("User with id {0} does not exist", id));
            }

            this.userSet.Remove(userToDelete);
            this.dbContext.SaveChanges();
        }

        public UserModel Get(int id)
        {
            return this.All().Where(um => um.Id == id).FirstOrDefault();
        }

        public IQueryable<UserModel> All()
        {
            var result = from user in this.userSet
                         select new UserModel()
                         {
                             Id = user.Id, 
                             Name = user.Name, 
                             AvatarURL = user.AvatarURL
                         };

            return result;
        }

        public IQueryable<UserModel> Find(System.Linq.Expressions.Expression<Func<UserModel, bool>> predicate)
        {
            return this.All().Where(predicate);
        }

        public UserModel Get(string username, string password)
        {
            User result = (from user in this.userSet
                           where user.Name.ToLower() == username.ToLower() && password == user.Password
                           select user).FirstOrDefault();

            if (result == null)
            {
                throw new ArgumentException("Wrong username or password");
            }

            if (result.SessionKey == null)
            {
                result.SessionKey = GenerateSessionKey(result.Id);
                this.dbContext.SaveChanges();
            }

            return new UserModel() { Id = result.Id, Name = result.Name, SessionKey = result.SessionKey, AvatarURL= result.AvatarURL };
        }

        public void Logout(string sessionKey)
        {
            User result = (from user in this.userSet
                           where user.SessionKey == sessionKey
                           select user).FirstOrDefault();

            if (result != null)
            {
                result.SessionKey = null;
                this.dbContext.SaveChanges();
            }
        }
    }
}