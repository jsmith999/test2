using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using Conta.UiController.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using XmlDal.ServiceHandler;

namespace Conta.Model {
    public partial class UiProject : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Project original;

        public UiProject(Project original)
            : base() {
            this.original = original;
        }

        [Required()]
        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [Required()]
        [StringLength(100)]
        public string Name {
            get { return original.Name; }
            set { SetProp(original.Name, value, v => original.Name = v, "Name"); }
        }

        public float Price {
            get { return original.Price; }
            set { SetProp(original.Price, value, v => original.Price = v, "Price"); }
        }

        [System.ComponentModel.DisplayName("Start Date")]
        public DateTime StartDate {
            get { return original.StartDate; }
            set { SetProp(original.StartDate, value, v => original.StartDate = v, "StartDate"); }
        }

        [System.ComponentModel.DisplayName("End Date")]
        public DateTime EndDate {
            get { return original.EndDate; }
            set { SetProp(original.EndDate, value, v => original.EndDate = v, "EndDate"); }
        }
        
        //[Browsable(false)]
        [LookupBoundProperty("UiProjectStatus", "Description", "Id", "Id")]
        public int StatusKey {
            get { return original.StatusKey; }
            set {
                SetProp(original.StatusKey, value, v => original.StatusKey = v, "StatusKey");
                //original.Status = XmlDal.DataContext.ProjectStatus.FromKey(value);
            }
        }
        /*
        [Browsable(false)]
        public ProjectStatus Status {
            get { return original.Status; }
            set { SetProp(original.Status, value, v => original.Status = v, "Status"); }
        }
        /* */
        [Browsable(false)]  // TODO : generate
        public int AddressKey {
            get { return original.AddressKey; }
            set {
                if (SetProp(original.AddressKey, value, v => original.AddressKey = v, "AddressKey")) {
                    Address = value == 0 ?
                        null :
                        XmlDal.DataContext.Addresss.FromKey(value);
                }
            }
        }

        //![Required()]
        public Address Address {
            get { return original.Address; }
            set { SetProp(original.Address, value, v => original.Address = v, "Address"); }
        }

        [StringLength(20)]
        public string Receivable {
            get { return original.Receivable; }
            set { SetProp(original.Receivable, value, v => original.Receivable = v, "Receivable"); }
        }

        [StringLength(20)]
        public string AgeOfReceivable {
            get { return original.AgeOfReceivable; }
            set { SetProp(original.AgeOfReceivable, value, v => original.AgeOfReceivable = v, "AgeOfReceivable"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Project, UiProject> {

            internal TheService() : base(XmlDal.DataContext.Projects, new KeyValuePair<string, Type>[] { /*add forward refs here*/ }) { }

            protected override Project GetOriginal(UiProject item) { return item.original; }

            protected override UiProject Create(Project original) { return new UiProject(original); }
        }
        #endregion
    }
}
