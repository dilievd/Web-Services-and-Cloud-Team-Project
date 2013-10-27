using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebChat.Repositories
{
    public interface IRepositoryUsers<T>
    {
        T Add(T entity);
        T Update(int id, T entity);
        void Delete(int id);
        T Get(int id);
        T Get(string username, string password);
        void Logout(string sessionKey);
        IQueryable<T> All();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
    }
}
