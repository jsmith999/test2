using Conta.Dal;
using Conta.DAL;
using Conta.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conta.UiController.Controller {
    public class DefaultCustomController : IBaseCustomController {
        protected readonly IBaseCustomView view;
        protected IDataClientService service;
        private bool isGlobalSearch;
        private bool hasParent;
        private string searchValue;
        private IUiBase parent;

        private IUiBase associatedParent;
        private IDisposable dataViewSourceHandler;

        public static ObservableCollection<UiProjectItemsCategory> ProjectData;

        public DefaultCustomController(IBaseCustomView view) {
            if (view == null)
                throw new ArgumentNullException("view");

            this.view = view;
            view.AddBOItem += view_AddBOItem;
            view.DelBOItem += view_DelBOItem;
            //view.ForwardListSelected += view_ForwardListSelected;
            view.MainGridSelectionChanged += view_MainGridSelectionChanged;
            view.StartSearch += view_StartSearch;

            this.SetViewDataSource = x => this.view.GridDataSource = x;
            dataViewSourceHandler = AppServices.Instance.DataViewSource.Register(DataViewSourceChanged);
        }

        void view_AddBOItem(object sender, EventArgs e) { this.AddNew(); }

        void view_DelBOItem(object sender, EventArgs e) { this.DeleteSelection(); }

        //void view_ForwardListSelected(object sender, EventArgs e) { SetDataType(sender as Type, service.SelectedItem as UiBase); }

        void view_MainGridSelectionChanged(object sender, UiBase e) { this.SelectionChanged(e); }

        void view_StartSearch(object sender, EventArgs e) { this.Search(this.SearchValue); }

        #region properties
        public string SearchValue {
            get { return searchValue ?? string.Empty; }
            set { SetValue(ref searchValue, value, "SearchValue"); }
        }

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
        public UiBase AddNew() {
            // update current, if necessary
            SelectionChanged(null);

            // add new item
            IUiBase newItem = null;
            try {
                newItem = service.Create();
            } catch (Exception ex) {
                Trace.TraceError("Add new item : " + ex.Message);
            }

            if (newItem == null) {
                view.ShowMessage("Add Item", "Could not create a new line", MessageActions.Ok);
                return null;
            }

            // select the new item
            var newIndex = service.GetIndex(newItem);
            Debug.Assert(newIndex >= 0);
            RefreshData();

            //SetDetailDataSourceIndex(newIndex);
            view.SelectedItem = newItem;
            return newItem as UiBase;
        }

        private void RefreshData() {
            SetDataSource(service.GetList(GetListParent(), SearchValue));
        }

        public void DeleteSelection() {
            var oldSelection = service.SelectedItem;
            SelectionChanged(null);
            var result = service.Delete(oldSelection);
            if (result == null)
                view.ShowMessage("Delete", "Could not delete the line", MessageActions.Ok);
            else
                RefreshData();
        }

        public bool CanClose() {
            CloseCurrentService();
            return true;
        }

        public virtual void SetDataType(Type type, UiBase parent) {
            CloseCurrentService();

            CurrentType = type;
            this.associatedParent = parent;

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
                SelectService(UiProject.Service, typeof(UiProject), parent, new[] { "Budgets" });
                return;
            }

            if (type == typeof(UiMaterial)) {
                UiMaterial.InitService();
                SelectService(UiMaterial.Service, typeof(UiMaterial), parent);
                return;
            }

            if (type == typeof(UiProjectCategory)) {
                UiProjectCategory.InitService();
                SelectService(UiProjectCategory.Service, typeof(UiProjectCategory), parent);
                return;
            }

            // TODO : throw an exception
        }

        public virtual void SelectionChanged(IUiBase item) {
            if (service.SelectedItem == item) return;   // re-selected the same item

            var oldSelection = service.SelectedItem;
            if (oldSelection != null) {
                oldSelection.PropertyChanged -= Selection_PropertyChanged;
                if (oldSelection.IsDirty) {
                    if (!oldSelection.Update()) {
                        view.ShowMessage("Delete", "Could not update the line", MessageActions.Ok);
                        // move selection back to it
                        view.SelectedItem = item;
                        return;
                    }
                }

                //view.SetRowStatus(service.GetIndex(oldSelection), oldSelection.IsLocked ? RowStatus.Locked : RowStatus.Normal);   // not for wpf
            }

            // set new selection
            service.SelectedItem = item;

            if (item == null) {
                view.SelectedItem = null;
                return;
            }

            // TODO pass the object, not the index
            //var index = service.GetIndex(item);
            //view.DetailDataSourceIndex = index;
            view.SelectedItem = item;
            item.PropertyChanged += Selection_PropertyChanged;
            view.GridReadOnly = item.IsLocked;
        }

        public void SetChildFilter(IUiBase parent) {
            this.SearchValue = string.Empty;
            this.parent = parent;
            HasParent = parent != null;
            var dataSource = service.GetList(GetListParent());
            RefreshService(dataSource);
        }

        public void Search(string searchValue) {
            //this.SearchValue = searchValue;
            var dataSource = service.GetList(GetListParent(), searchValue);
            RefreshService(dataSource);
        }

        public void ExecuteReport(string header) {
            // TODO : change the view to report view
            //if (header == "Budgets")
            //    view.ShowReport(new BudgetReport().Create());
        }
        #endregion

        protected void DataViewSourceChanged(DataViewParameter parameter) {
            if (service == null) {
                this.service = AppServices.Instance.GetDataService(parameter.BusinessObject.UiDataType);
                CurrentType = parameter.BusinessObject.UiDataType;
                //this.associatedParent = parameter.Filter;
                view.GridDataSource = this.service.GetList();
                if (parameter.Filter != null)
                    SetChildFilter(parameter.Filter);
            } else {
                CloseCurrentService();
                dataViewSourceHandler.Dispose();
            }
        }

        #region helpers
        protected IUiBase GetListParent() {
            return this.parent != null ?
                this.parent :
                this.IsGlobalSearch ? null : this.associatedParent;
        }

        protected virtual void SetDataSource(ICollection dataSource) {
            //view.GridDetailSource = dataSource;
            SetViewDataSource(dataSource);
        }

        private void SelectService(IDataClientService newService, Type dataType, UiBase parent, IEnumerable<string> reports = null) {
            // reset filter
            this.SearchValue = string.Empty;

            service = newService;
            service.UpdateStatus += service_UpdateStatus;
            //view.SetDetail(dataType);/* no need : detail items bound directly to grid's selection */
            var dataSource = service.GetList(GetListParent());
            RefreshService(dataSource);
            RaisePropertyChanged("ForwardLinks");
            HasParent = parent != null;

            // TODO
            //if (reports != null)
            //    view.SetReports(reports);

            // TODO : remove temporary hack
            if (dataType == typeof(UiEmployee))
                (dataSource as ObservableCollection<UiEmployee>)[1].IsLocked = true;
        }

        private void RefreshService(ICollection dataSource) {
            SetDataSource(dataSource);
            SetStatus(dataSource);
        }

        private void SetStatus(ICollection dataSource) {
            // not in WPF
            //int index = 0;
            //foreach (UiBase item in dataSource)
            //    view.SetRowStatus(index++, item.IsLocked ? RowStatus.Locked : RowStatus.Normal);
        }

        public void CloseCurrentService() {
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
            //view.SetRowStatus(e.Index, e.IsLocked ? RowStatus.Locked : RowStatus.Normal);   // built-in by design 
        }

        private void Selection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var crtSelection = sender as IUiBase;
            var index = service.GetIndex(crtSelection);
            // built-in by design :
            //if (crtSelection.IsDirty)
            //    view.SetRowStatus(index, RowStatus.Editing);
            //else
            //    view.SetRowStatus(index, RowStatus.Normal);
        }

        //private void SetDetailDataSourceIndex(int index) {
        //    view.DetailDataSourceIndex = index;
        //}

        protected bool SetValue<T>(ref T current, T newValue, string propName) {
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
