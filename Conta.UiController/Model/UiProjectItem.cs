#define DetailsAsBindingList
using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using XmlDal.ServiceHandler;

namespace Conta.Model {
    public class UiProjectItemsCategory : UiBase {
        #region Service
        private static ProjectItemsCategoryService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new ProjectItemsCategoryService();
        }
        #endregion

        private readonly ProjectItemCategory original;
#if(DetailsAsBindingList)
        private BindingList<UiProjectItemDetail> details;
#else
        private ObservableCollection<UiProjectItemDetail> details;
#endif
        public UiProjectItemsCategory(ProjectItemCategory original) : base() { this.original = original; }

        /*
        public override bool HasDetails {
            get { return base.HasDetails; }
            set {
                base.HasDetails = value;

                foreach (var item in MainController.ProjectData)
                    if (item.Parent == this.Parent && item.Order != 0) {
                        //item.RaisePropertyChanged("");
                        Debug.Assert(item is UiProjectItemDetail);
                        (item as UiProjectItemDetail).IsRowVisible = !value;
                    }
                RaisePropertyChanged("HasDetails");
            }
        }
        /* */

        // no Notification & validation : R/O
        [Browsable(false)]
        public int Key { get { return original.Key; } }

        public string Name { get { return IsDeleted ? /*string.Empty*/ "?" : original.Name; } }

        public double Value { get; private set; }

        [StringLength(100)]
        public string Observations {
            get { return original.Observations; }
            set { SetProp(original.Observations, value, v => original.Observations = v, "Observations"); }
        }

        [Browsable(false)]
        public bool IsDeleted { get { return original.IsDeleted; } }

        private double nameWidth;
        [Browsable(false)]
        public double NameWidth {
            get { return nameWidth; }
            set { SetProp(nameWidth, value, v => nameWidth = v, "NameWidth"); }
        }
#if(DetailsAsBindingList)
        [Browsable(false)]
        public BindingList<UiProjectItemDetail> Details {
            get {
                return details;
            }

            private set {
                if (details != null) {
                    details.ListChanged -= Details_ListChanged;
                    foreach (var item in details)
                        item.PropertyChanged -= Detail_PropertyChanged;
                }

                details = value;
                //Debug.WriteLine("Recalculate : Details change");
                Recalculate();
                RaisePropertyChanged("Details");

                if (details != null) {
                    details.ListChanged += Details_ListChanged;
                    foreach (var item in details)
                        item.PropertyChanged += Detail_PropertyChanged;
                }

            }
        }

        private void Details_ListChanged(object sender, ListChangedEventArgs e) {
            //Debug.WriteLine("Recalculate : Details change " + e.ListChangedType);
            Recalculate();
        }

        private void Detail_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Quantity" ||
                e.PropertyName == "UnitPrice") {
                //Debug.WriteLine("Recalculate : PropertyChanged change : "+ e.PropertyName);
                Recalculate();
            }
        }
#else
        [Browsable(false)]
        public ObservableCollection<UiProjectItemDetail> Details {
            get {
                return details;
            }

            private set {
                if (details != null) {
                    details.CollectionChanged -= details_CollectionChanged;
                    foreach (var item in details)
                        item.PropertyChanged -= Detail_PropertyChanged;
                }

                details = value;
                //Debug.WriteLine("Recalculate : Details change");
                Recalculate();
                RaisePropertyChanged("Details");

                if (details != null) {
                    details.CollectionChanged += details_CollectionChanged;
                    foreach (var item in details)
                        item.PropertyChanged += Detail_PropertyChanged;
                }
            }
        }

        private void details_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            Debug.WriteLine("Recalculate : Details change " + e.Action);
            Recalculate();
        }

        private void Detail_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Quantity" ||
                e.PropertyName == "UnitPrice") {
                //Debug.WriteLine("Recalculate : PropertyChanged change : "+ e.PropertyName);
                Recalculate();
            }
        }
#endif

        [Browsable(false)]
        public object SelectedDetail { get; set; }

        private void Recalculate() {
            var total = 0d;
            if (details != null)
                foreach (var item in details)
                    total += item.Value;

            this.Value = total;
            RaisePropertyChanged("Value");
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class ProjectItemsCategoryService : BaseUiService<ProjectItemCategory, UiProjectItemsCategory> {
            internal ProjectItemsCategoryService() : base(XmlDal.DataContext.ProjectItemsCategory, new KeyValuePair<string, Type>[] { }) { }

            public override int GetIndex(IUiBase item) {
                if (item is UiProjectItemsCategory)
                    return base.GetIndex(item);
                else
                    return UiProjectItemDetail.Service.GetIndex(item);
            }

            public override ICollection GetList(IUiBase parent, string searchValue = null) {
                if (parent == null || !(parent is UiProject))
                    return new List<ProjectItemCategory>();

                var result = base.GetList(parent);
                var projectKey = (parent as UiProject).Id;
                foreach (UiProjectItemsCategory item in cache)
                    item.Details = (UiProjectItemDetail.Service as UiProjectItemDetail.ProjectItemDetailService).GetList(projectKey, item.original.Key);   // TODO : pass the project!
                    //item.Details = new ObservableCollection<UiProjectItemDetail>((UiProjectItemDetail.Service as UiProjectItemDetail.ProjectItemDetailService).GetList(projectKey, item.original.Key));   // TODO : pass the project!
                return result;
            }

            public override ICollection GetList(LambdaExpression where = null, string toSearch = null) {
                throw new NotImplementedException();  // do not use! (missing project parameter)
            }

            protected override ProjectItemCategory GetOriginal(UiProjectItemsCategory item) {
                if (item == null) throw new ArgumentNullException("original");

                return item.original;
            }

            protected override UiProjectItemsCategory Create(ProjectItemCategory original) {
                if (original == null) throw new ArgumentNullException("original");

                return new UiProjectItemsCategory(original);
            }
        }
        #endregion
    }

    public class UiProjectItemDetail : UiBase {
        #region Service
        public static IDataClientService Service = new ProjectItemDetailService();
        #endregion

        public readonly ProjectItemDetailMaterial original;

        public UiProjectItemDetail(ProjectItemDetailMaterial original) : base() { this.original = original; }

        #region Properties
        [Browsable(false)]
        public int Key { get { return original.Key; } }

        [ReadOnly(true)]
        public string Name { get { return original.Name; } }

        [ReadOnly(true)]
        public string MeasuringUnit { get { return this.original.MeasuringUnit; } }

        public double Quantity {
            get {
                return original.Quantity;
            }

            set {
                //Debug.WriteLine("Quantity : {0} -> {1}", original.Quantity, value);
                if (SetProp(original.Quantity, value, v => original.Quantity = v, "Quantity"))
                    RaisePropertyChanged("Value");
            }
        }

        public double UnitPrice {
            get {
                return original.Material.UnitPrice;
            }

            set {
                if (SetProp(original.Material.UnitPrice, value, v => original.Material.UnitPrice = v, "UnitPrice"))
                    RaisePropertyChanged("Value");
            }
        }

        public double Value { get { return UnitPrice * Quantity; } }

        [StringLength(20)]
        public string Observations {
            get { return original.Observations; }
            set { SetProp(original.Observations, value, v => original.Observations = v, "Observations"); }
        }

        //[Browsable(false)]
        //public int Category { get { return original.Category; } }

        //[Browsable(false)]
        //public int MaterialKey { get { return original.Material.Key; } }
        [Browsable(false)]
        public UiMaterial Material {
            //get { return original.Material; }
            set {
                original.Material = value.original;
                original.MaterialKey = value.original.Key;
            }
        }
        #endregion

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        internal class ProjectItemDetailService : BaseUiService<ProjectItemDetailMaterial, UiProjectItemDetail> {
            internal ProjectItemDetailService() : base(XmlDal.DataContext.ProjectItemDetail, new KeyValuePair<string, Type>[] { }) { }

            public BindingList<UiProjectItemDetail> GetList(int project, int category) {
                var originals = (this.service as ProjectItemMaterialServiceHandler).GetList(project, category);
                var result = new BindingList<UiProjectItemDetail>();
                foreach (var orig in originals)
                    result.Add(Create(orig));

                return result;
            }

            protected override ProjectItemDetailMaterial GetOriginal(UiProjectItemDetail item) {
                if (item == null) throw new ArgumentNullException("original");

                return item.original;
            }

            protected override UiProjectItemDetail Create(ProjectItemDetailMaterial original) {
                if (original == null) throw new ArgumentNullException("original");

                return new UiProjectItemDetail(original);
            }
        }
        #endregion
    }
}
