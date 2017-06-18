using Conta.Dal;
using Conta.DAL;
using Conta.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Conta.UiController.Controller {
    public class TypeEventArgs : EventArgs {
        public TypeEventArgs(Type type) {
            this.Type = type;
        }

        public Type Type { get; private set; }
    }

    public enum RowStatus {
        Normal,
        Editing,
        Locked,
    }

    public interface IMainView {
        object GridDataSource { set; }
        object GridDetailSource { set; }
        bool GridReadOnly { set; }
        int DetailDataSourceIndex { set; }
        void SetRowStatus(int index, RowStatus status);
        void SetDetail(Type type);
    }

    public class MainController : BaseController {

        public MainController(IMainView view) : base(view) { }

        public IController DetailController { get; private set; }

        public override void SetDataType(Type type, UiBase parent) {
            if (type == typeof(UiProject)) {
                DetailController = new BaseController(view);
                DetailController.SetViewDataSource = x => this.view.GridDetailSource = x;
                DetailController.SetDataType(typeof(UiProjectItemsCategory), parent);
            } else {
                if (DetailController != null)
                    view.GridDetailSource = null;
                DetailController = null;
            }

            base.SetDataType(type, parent);
        }

        public override void SelectionChanged(IUiBase item) {
            if (DetailController != null) {
                // TODO : make it more generic
                Debug.Assert(this.CurrentType == typeof(UiProject));
                if (item == null)
                    view.GridDetailSource = null;   // hide details when not selected
                else
                    DetailController.SetChildFilter(item);
            }

            base.SelectionChanged(item);
        }

        protected override void SetDataSource(ICollection dataSource) {
            view.GridDataSource = dataSource;
        }
    }

    public class BaseController : IController {
        protected readonly IMainView view;
        private IDataClientService service;
        private bool isGlobalSearch;
        private bool hasParent;
        private IUiBase parent;

        public static ObservableCollection<UiProjectItemsCategory> ProjectData;

        public BaseController(IMainView view) {
            if (view == null)
                throw new ArgumentNullException("view");

            this.view = view;
            this.SetViewDataSource = x => this.view.GridDataSource = x;
        }

        #region properties
        public Type CurrentType { get; private set; }

        public Action<ICollection> SetViewDataSource { get; set; }

        public bool IsGlobalSearch { 
            get { return isGlobalSearch; }
            set { SetValue(ref isGlobalSearch, value, "IsGlobalSearch"); }
        }

        public bool HasParent {
            get { return hasParent; }
            set { SetValue(ref hasParent, value, "HasParent"); }
        }

        public IEnumerable<KeyValuePair<string, Type>> ForwardLinks { get { return service == null ? null : service.ChildrenTypes; } }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region API
        public void AddNew() {
            // update current, if necessary
            SelectionChanged(null);

            // add new item
            var newItem = service.Create();

            // select the new item
            var newIndex = service.GetIndex(newItem);
            Debug.Assert(newIndex >= 0);
            SetDataSource(service.GetList());  // TODO : track filter, if any
            SetDetailDataSourceIndex(newIndex);
        }

        public bool CanClose() {
            CloseCurrentService();
            return true;
        }

        public virtual void SetDataType(Type type, UiBase parent) {
            CloseCurrentService();

            CurrentType = type;

            if (type == typeof(UiClient)) {
                UiClient.InitService();
                SelectService(UiClient.Service, typeof(UiClient), parent);
                return;
            }

            if (type == typeof(UiEmployee)) {
                UiEmployee.InitService();
                SelectService(UiEmployee.Service, typeof(UiEmployee), parent);
                return;
            }

            if (type == typeof(UiProjectItemsCategory)) {
                // only as DetailController
                UiProjectItemsCategory.InitService();
                SelectService(UiProjectItemsCategory.Service, typeof(UiProjectItemsCategory), parent);
                return;
            }

            if (type == typeof(UiProject)) {
                UiProject.InitService();
                SelectService(UiProject.Service, typeof(UiProject), parent);
                return;
            }

            // TODO : throw an exception
        }

        public virtual void SelectionChanged(IUiBase item) {
            if (service.SelectedItem == item) return;

            var oldSelection = service.SelectedItem;
            if (oldSelection != null) {
                oldSelection.PropertyChanged -= Selection_PropertyChanged;
                if (oldSelection.IsDirty) {
                    if (!oldSelection.Update()) {
                        // TODO
                        // show message "Could not update"
                        // move selection back to it
                    }
                }

                view.SetRowStatus(service.GetIndex(oldSelection), oldSelection.IsLocked ? RowStatus.Locked : RowStatus.Normal);
            }

            service.SelectedItem = item;

            if (item == null) {
                view.DetailDataSourceIndex = -1;
                return;
            }

            var index = service.GetIndex(item);
            view.DetailDataSourceIndex = index;
            item.PropertyChanged += Selection_PropertyChanged;
            view.GridReadOnly = item.IsLocked;
        }

        public void SetChildFilter(IUiBase parent) {
            this.parent = parent;
            HasParent = parent != null;
            var dataSource = service.GetList(parent);
            RefreshService(dataSource);
        }

        public void Search(string searchValue) {
            var dataSource = service.GetList(parent, searchValue);
            RefreshService(dataSource);
        }
        #endregion

        #region helpers
        protected virtual void SetDataSource(ICollection dataSource) {
            //view.GridDetailSource = dataSource;
            SetViewDataSource(dataSource);
        }

        private void SelectService(IDataClientService newService, Type dataType, UiBase parent) {
            service = newService;
            service.UpdateStatus += service_UpdateStatus;
            view.SetDetail(dataType);
            var dataSource = service.GetList(parent);
            RefreshService(dataSource);
            RaisePropertyChanged("ForwardLinks");
            HasParent = parent != null;

            // TODO : remove temporary hack
            if (dataType == typeof(UiEmployee))
                (dataSource as ObservableCollection<UiEmployee>)[1].IsLocked = true;
        }

        private void RefreshService(ICollection dataSource) {
            SetDataSource(dataSource);
            SetStatus(dataSource);
        }

        private void SetStatus(ICollection dataSource) {
            int index = 0;
            foreach (UiBase item in dataSource)
                view.SetRowStatus(index++, item.IsLocked ? RowStatus.Locked : RowStatus.Normal);
        }

        private void CloseCurrentService() {
            if (service == null) return;

            // update selection, if changed
            if (service.SelectedItem != null &&
                service.SelectedItem.IsDirty) {
                SelectionChanged(null);     // save current
            }

            // remove service events
            service.UpdateStatus -= service_UpdateStatus;
        }

        private void service_UpdateStatus(object sender, ChangeStatusEventArgs e) {
            var item = sender as UiBase;
            if (item == null) return;   // should not happen

            //if (item == service.SelectedItem) return;
            view.SetRowStatus(e.Index, e.IsLocked ? RowStatus.Locked : RowStatus.Normal);
        }

        private void Selection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var crtSelection = sender as IUiBase;
            var index = service.GetIndex(crtSelection);
            if (crtSelection.IsDirty)
                view.SetRowStatus(index, RowStatus.Editing);
            else
                view.SetRowStatus(index, RowStatus.Normal);
        }

        private void SetDetailDataSourceIndex(int index) {
            view.DetailDataSourceIndex = index;
        }

        private bool SetValue<T>(ref T current, T newValue, string propName) {
            if (object.Equals(current, newValue)) 
                return false;

            current = newValue;
            RaisePropertyChanged(propName);
            return true;
        }

        private void RaisePropertyChanged(string propName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
