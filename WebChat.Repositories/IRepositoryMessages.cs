using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChat.Repositories
{
    public interface IRepositoryMessages<T>
    {
        IEnumerable<T> Get(int chatId, string sessionKey);
    }
}
