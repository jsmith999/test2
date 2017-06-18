using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using XmlDal.ServiceHandler;

namespace Conta.Model {
    /*
    public interface IUiProjectItem : IUiBase {
        bool HasDetails { get; set; }
        int Key { get; }
        string MeasuringUnit { get; set; }
        string Name { get; set; }
        string Observations { get; set; }
        int Order { get; set; }
        int Parent { get; }
        double Quantity { get; set; }
        double UnitPrice { get; set; }
        bool IsRowVisible { get; }
    }
    
    public abstract class UiProjectItem : UiBase, IUiProjectItem {
        #region Service
        private static ProjectItemService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new ProjectItemService();
        }
        #endregion

        protected readonly IUniformProjectGrid original;
        protected UiProjectItem(IUniformProjectGrid original) { this.original = original; }

        #region properties
        [Browsable(false)]
        public abstract int Key { get; }

        [Browsable(false)]
        bool isRowVisible = true;
        public virtual bool IsRowVisible { get { return isRowVisible; } set { isRowVisible = value; } }

        private bool hasDetails;
        public virtual bool HasDetails {
            get { return hasDetails; }
            set { hasDetails = value; }
        }

        [ReadOnly(true)]
        public virtual string Name {
            get { return original.Name; }
            set { }
            //set { SetProp(original.Name, value, v => original.Name = v, "Name"); }
        }

        [ReadOnly(true)]
        public virtual string MeasuringUnit {
            get { return this.original.MeasuringUnit; }
            set { }
        }

        public virtual double Quantity {
            get { return original.Quantity; }
            set { SetProp(original.Quantity, value, v => original.Quantity = v, "Quantity"); }
        }

        public virtual double UnitPrice {
            get { return original.UnitPrice; }
            set { SetProp(original.UnitPrice, value, v => original.UnitPrice = v, "UnitPrice"); }
        }

        public virtual double Value { get { return 0d; } }

        public virtual string Observations {
            get { return original.Observations; }
            set { SetProp(original.Observations, value, v => original.Observations = v, "Observations"); }
        }

        [Browsable(false)]
        public virtual int Parent { get { return original.Parent; } }

        [Browsable(false)]
        public virtual int Order {
            get { return original.Order; }
            set { SetProp(original.Order, value, v => original.Order = v, "Order"); }
        }
        #endregion

        protected override IDataClientService GetService() { return Service; }

        #region service implementation
        class ProjectItemService : BaseUiService<IUniformProjectGrid, IUiProjectItem> {
            internal ProjectItemService() : base(XmlDal.DataContext.ProjectItem) { }

            public override System.Collections.ICollection GetList(string toSearch) {
                var result = base.GetList(toSearch);

                foreach (var item in cache)
                    if (item is UiProjectItemDetail) {
                        var itemDetail = item as UiProjectItemDetail;
                        itemDetail.CategoryObject = cache.FirstOrDefault(x => (x as IUiProjectItem).Parent == itemDetail.Category && (x as IUiProjectItem).Order == 0) as UiProjectItemsCategory;
                    }
                return result;
            }

            protected override IUniformProjectGrid GetOriginal(IUiProjectItem item) {
                if (item == null) throw new ArgumentNullException("original");

                if (item is UiProjectItemsCategory) return (item as UiProjectItemsCategory).original;
                if (item is UiProjectItemDetail) return (item as UiProjectItemDetail).original;

                throw new NotImplementedException();
            }

            protected override IUiProjectItem Create(IUniformProjectGrid original) {
                if (original == null) throw new ArgumentNullException("original");

                if (original is ProjectItemCategory) return new UiProjectItemsCategory(original as ProjectItemCategory);
                if (original is ProjectItemDetailMaterial) return new UiProjectItemDetail(original as ProjectItemDetailMaterial);

                throw new NotImplementedException();
            }
        }
        #endregion
    }
    /* */
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

        public double Value { get { return 0d; } }  // TODO : calculate

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

        [Browsable(false)]
        public ICollection /*IList<UiProjectItemDetail>*/ Details { get; private set; }

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

        private readonly ProjectItemDetailMaterial original;

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

        public string Observations {
            get { return original.Observations; }
            set { SetProp(original.Observations, value, v => original.Observations = v, "Observations"); }
        }

        //[Browsable(false)]
        //public int Category { get { return original.Category; } }

        //[Browsable(false)]
        //public int MaterialKey { get { return original.Material.Key; } }
        #endregion

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        internal class ProjectItemDetailService : BaseUiService<ProjectItemDetailMaterial, UiProjectItemDetail> {
            internal ProjectItemDetailService() : base(XmlDal.DataContext.ProjectItemDetail, new KeyValuePair<string, Type>[] { }) { }

            public List<UiProjectItemDetail> GetList(int project, int category) {
                var originals = (this.service as ProjectItemMaterialServiceHandler).GetList(project, category);
                var result = new List<UiProjectItemDetail>();
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
