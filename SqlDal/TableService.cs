using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlDal {
    class TableService<TTable, TKey> : Conta.DAL.BaseTableService<TTable, TKey> where TTable : class {
        protected override IEnumerable<TTable> DoGetList(LambdaExpression where, string toSearch) {
            throw new NotImplementedException();
        }

        protected override TTable DoCreate() {
            throw new NotImplementedException();
        }

        protected override bool DoUpdate(TTable item) {
            throw new NotImplementedException();
        }

        protected override TTable DoDelete(TTable item) {
            throw new NotImplementedException();
        }

        public override TKey GetKeyValue(TTable item) {
            throw new NotImplementedException();
        }
    }
}
