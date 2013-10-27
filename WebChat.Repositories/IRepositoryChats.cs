using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebChat.Repositories
{
    public interface IRepositoryChats<T>
    {
        T New(int id, string sessionKey);
        T Get(int id, string sessionKey);
        void SendMessage(int id, string sessionKey, string content);
        IQueryable<T> All(string sessionKey);
    }
}
