using System.Collections.Generic;
using Conta.Dal.Feedback;
using System.Linq.Expressions;

namespace Conta.DAL {
    public interface ITableService<T> /*: ITableService*/ where T : class {
        IEnumerable<T> GetList(LambdaExpression where, string toSearch);
        IEnumerable<T> GetList(object parent, string toSearch);
        T Create();
        bool Update(T item);
        T Delete(T item);
        bool Lock(T item, bool locked);
        bool AreEqual(T left, T right);
        BroadcastService Broadcaster { get; }
    }
}
