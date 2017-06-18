using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public class UiProject : UiBase {
        #region Service
        private static ProjectService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new ProjectService();
        }
        #endregion

        internal readonly Project original;

        public UiProject(Project original)
            : base() {
            this.original = original;
        }

        #region properties
        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [DisplayName("Project Name")]
        [StringLength(20)]
        public string Name {
            get { return original.Name; }
            set { SetProp(original.Name, value, x => original.Name = x, "Name"); }
        }

        public double Budget {
            get { return original.Budget; }
            set { SetProp(original.Budget, value, x => original.Budget = x, "Budget"); }
        }

        [Browsable(false)]
        public int ClientKey { get { return original.ClientKey; } }
        #endregion

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class ProjectService : BaseUiService<Project, UiProject> {

            internal ProjectService() : base(XmlDal.DataContext.Projects, new KeyValuePair<string, Type>[] { }) { }

            protected override Project GetOriginal(UiProject item) { return item.original; }

            protected override UiProject Create(Project original) { return new UiProject(original); }
        }
        #endregion
    }
}
