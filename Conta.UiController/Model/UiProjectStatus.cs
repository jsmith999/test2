using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiProjectStatus : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly ProjectStatus original;

        public UiProjectStatus(ProjectStatus original)
            : base() {
            this.original = original;
        }

        public int Id { get { return original.Id; } }

        [StringLength(20)]
        [Required()]
        public string Description {
            get { return original.Description; }
            set { SetProp(original.Description, value, v => original.Description = v, "Description"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<ProjectStatus, UiProjectStatus> {

            internal TheService() : base(XmlDal.DataContext.ProjectStatus, new KeyValuePair<string, Type>[] { /*add forward refs here*/ }) { }

            protected override ProjectStatus GetOriginal(UiProjectStatus item) { return item.original; }

            protected override UiProjectStatus Create(ProjectStatus original) { return new UiProjectStatus(original); }
        }
        #endregion
    }
}
