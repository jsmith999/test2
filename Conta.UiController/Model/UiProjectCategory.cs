using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public class UiProjectCategory : UiBase {
        #region Service
        private static ThisService service;

        public static IDataClientService Service { get { return service; } }

        public static IDataClientService ServiceCreator() { return service = new ThisService(); }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new ThisService();
        }
        #endregion

        internal readonly ProjectItemCategory original;

        public UiProjectCategory(ProjectItemCategory original)
            : base() {
            this.original = original;
        }

        #region properties
        [Browsable(false)]
        public int Key { get { return original.Key; } }
        [StringLength(20)]
        public string Name {
            get { return original.Name; }
            set { SetProp(original.Name, value, x => original.Name = x, "Name"); }
        }
        [Browsable(false)]
        public bool IsDeleted { get; set; }
        [StringLength(100)]
        public virtual string Observations {
            get { return original.Observations; }
            set { SetProp(original.Observations, value, x => original.Observations = x, "Observations"); }
        }
        #endregion

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class ThisService : BaseUiService<ProjectItemCategory, UiProjectCategory> {

            internal ThisService() : base(XmlDal.DataContext.ProjectItemsCategory, new KeyValuePair<string, Type>[] { }) { }

            protected override ProjectItemCategory GetOriginal(UiProjectCategory item) { return item.original; }

            protected override UiProjectCategory Create(ProjectItemCategory original) { return new UiProjectCategory(original); }
        }
        #endregion
    }
}
