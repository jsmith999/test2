using Conta.Dal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Conta.DAL {
    public class ChangeStatusEventArgs : EventArgs {
        public ChangeStatusEventArgs(int index,
            bool isLocked) {

            Index = index;
            IsLocked = isLocked;
        }

        public int Index { get; private set; }
        //public object Item { get; private set; }
        public bool IsLocked { get; private set; }
    }

    public interface IDataClientService {
        IUiBase SelectedItem { get; set; }

        // key : type name; value : display name
        IEnumerable<KeyValuePair<string, Type>> ChildrenTypes { get; }

        event EventHandler<ChangeStatusEventArgs> UpdateStatus;

        ICollection GetList(IUiBase parent, string searchValue = null);
        ICollection GetList(LambdaExpression where = null, string toSearch= null);
        IUiBase Create();
        bool Update(IUiBase item);
        IUiBase Delete(IUiBase item);
        bool Lock(IUiBase item, bool locked);

        //IUiBase GetSelected();
        int GetIndex(IUiBase item);
        object GetOriginal(object item);
    }
}
