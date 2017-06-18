using Conta.Dal.Feedback;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Conta.Dal;

namespace Conta.DAL {
    public abstract class BaseTableService<TTable, TKey> : ITableService<TTable>
            where TTable : class {
        protected string TableName;
        protected string KeyName;

        public BaseTableService() {
            Broadcaster = new BroadcastService();
            // TODO : add logger
        }

        #region ITableService
        public virtual IEnumerable<TTable> GetList(LambdaExpression where = null, string toSearch = null) {
            Debug.Write("DoGetList");
            try {
                return DoGetList(where, toSearch) as IEnumerable<TTable>;
            } finally {
                Debug.WriteLine(".");
            }
        }

        public virtual IEnumerable<TTable> GetList(object parent, string searchValue = null) {
            var filter = new ParentFilter<TTable>(parent);
            var unfiltered = GetList(null, searchValue) as IEnumerable<TTable>;
            var result = unfiltered.Where(x => filter.Filter(x));   // TODO : move to DAL
            return result;
        }

        public TTable Create() {
            Debug.Write("Create");
            try {
                var result = DoCreate();
                Broadcaster.RaiseStatusChange(DalObjectStatus.Normal, result);
                return result;
            } finally {
                Debug.WriteLine(".");
            }
        }

        public bool Update(TTable item) {
            Debug.Write("Update");
            try {
                var result = DoUpdate(item);
                Broadcaster.RaiseStatusChange(DalObjectStatus.Updated, result);
                return result;
            } finally {
                Debug.WriteLine(".");
            }
        }

        public TTable Delete(TTable item) {
            Debug.Write("Delete");
            try {
                var result = DoDelete(item);
                Broadcaster.RaiseStatusChange(DalObjectStatus.Updated, result);
                return result;
            } finally {
                Debug.WriteLine(".");
            }
        }

        public bool Lock(TTable item, bool locked) {
            Debug.Write("Lock " + locked.ToString());
            try {
                var result = locked;  // TODO
                Broadcaster.RaiseStatusChange(locked ? DalObjectStatus.Locked : DalObjectStatus.Normal, item);
                return result;
            } finally {
                Debug.WriteLine(".");
            }
        }

        public bool AreEqual(TTable left, TTable right) { return object.Equals(GetKeyValue(left), GetKeyValue(right)); }

        public BroadcastService Broadcaster { get; private set; }
        #endregion

        #region specifics
        protected abstract IEnumerable<TTable> DoGetList(LambdaExpression where, string toSearch);

        protected abstract TTable DoCreate();

        protected abstract bool DoUpdate(TTable item);

        protected abstract TTable DoDelete(TTable item);

        public abstract TKey GetKeyValue(TTable item);
        #endregion
    }
}
