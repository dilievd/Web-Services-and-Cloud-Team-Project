using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChat.Repositories.SerializableModels
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PassWord { get; set; }

        public string SessionKey { get; set; }

        public string AvatarURL { get; set; }
    }
}
