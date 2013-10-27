using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChat.Repositories.SerializableModels
{
    public class SerializableMessage
    {
        public int OwnerId { get; set; }

        public string OwnerName { get; set; }

        public string Content { get; set; }
    }
}
