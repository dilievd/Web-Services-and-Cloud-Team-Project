using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChat.Repositories.SerializableModels
{
    public class ChatModel
    {
        public int Id { get; set; }

        public string Channel { get; set; }

        public UserModel User1 { get; set; }

        public UserModel User2 { get; set; }
    }
}
