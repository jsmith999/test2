using Conta.Dal;
using Conta.Dal.Feedback;
using Conta.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace Conta.Model {
    abstract class BaseUiService<TDal, TUiItem> : IDataClientService
        where TDal : class
        where TUiItem : class, IUiBase {

        protected readonly ITableService<TDal> service;
        protected ObservableCollection<TUiItem> cache;

        protected BaseUiService(ITableService<TDal> service,
            IEnumerable<KeyValuePair<string, Type>> childrenTypes) {
            this.service = service;
            this.ChildrenTypes = childrenTypes;

            SelectedItem = null;
            service.Broadcaster.StatusChange += Broadcaster_StatusChange;
        }

        void Broadcaster_StatusChange(object sender, Conta.Dal.Feedback.BroadcastEventArgs e) {
            var item = e.DalObject as TDal;
            if (item == null) return;   // wrong type

            var index = GetIndex(item);
            if (index < 0) return;  // not visible

            if (e.Status == DalObjectStatus.Locked ||
                e.Status == DalObjectStatus.Normal)
                RaiseUpdateStatus(index, e.Status == DalObjectStatus.Locked);
        }

        private void RaiseUpdateStatus(int index, bool isLocked) {
            if (UpdateStatus != null)
                UpdateStatus(cache[index], new ChangeStatusEventArgs(index, isLocked));
        }

        #region IDataClientService<TUiItem> Members
        public IUiBase SelectedItem { get; set; }
        public IEnumerable<KeyValuePair<string, Type>> ChildrenTypes { get; private set; }

        public event EventHandler<ChangeStatusEventArgs> UpdateStatus;

        public virtual ICollection GetList(IUiBase parent, string searchValue = null)
        {
            return GetList(service.GetList(parent == null ? null : (parent as UiBase).GetService().GetOriginal(parent), searchValue));
        }

        public virtual ICollection GetList(LambdaExpression where = null, string toSearch = null)
        {
            var dalList = service.GetList(where, toSearch);
            return GetList(dalList);
        }

        protected ICollection GetList(IEnumerable<TDal> dalList)
        {
            // dispose of the old list
            if (cache!=null)
                cache.CollectionChanged -= Cache_CollectionChanged;

            cache = new ObservableCollection<TUiItem>();
            if (dalList != null) {
                foreach (var item in dalList) {
                    var uiItem = Create(item);
                    cache.Add(uiItem);
                }
            }

            cache.CollectionChanged += Cache_CollectionChanged; // skip the initialization
            return cache;
        }

        private void Cache_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (TUiItem item in e.OldItems)
                        this.Delete(item);
                    break;
            }
        }

        public IUiBase Create() {
            var result = Create(service.Create());
            cache.Add(result);
            return result;
        }

        public bool Update(IUiBase item) {
            //var index = GetIndex(item);
            var updated = service.Update(GetOriginal(item as TUiItem));

            //item.IsDirty = false;
            item.IsLocked = !updated;
            return updated;
        }

        public IUiBase Delete(IUiBase item) {
            return service.Delete(GetOriginal(item as TUiItem)) == null ?
                null :
                item;
        }

        public bool Lock(IUiBase item, bool locked) {
            return !(item is TUiItem) || service.Lock(GetOriginal(item as TUiItem), locked);
        }

        //public IUiBase GetSelected() { return SelectedIndex < 0 ? null : cache[SelectedIndex]; }

        public virtual int GetIndex(IUiBase item) {
            var uiItem = item as TUiItem;
            if (uiItem == null) return -1;
            var search = GetOriginal(uiItem);
            return GetIndex(search);
        }

        private int GetIndex(TDal item) {
            if (cache != null) {
                for (int result = 0; result < cache.Count; result++)
                    if (service.AreEqual(item, GetOriginal(cache[result] as TUiItem)))
                        return result;
            }

            return -1;
        }
        #endregion

        public object GetOriginal(object item) { return GetOriginal(item as TUiItem); }

        protected abstract TDal GetOriginal(TUiItem item);

        protected abstract TUiItem Create(TDal original);
    }
}
