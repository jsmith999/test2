using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiPayment : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Payment original;

        public UiPayment(Payment original)
            : base() {
            this.original = original;
        }

        [Required()]
        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [Required()]
        public int Project {
            get { return original.Project; }
            set { SetProp(original.Project, value, v => original.Project = v, "Project"); }
        }

        [Required()]
        public DateTime Date {
            get { return original.Date; }
            set { SetProp(original.Date, value, v => original.Date = v, "Date"); }
        }

        [Required()]
        public float Sum {
            get { return original.Sum; }
            set { SetProp(original.Sum, value, v => original.Sum = v, "Sum"); }
        }

        [StringLength(100)]
        public string Description {
            get { return original.Description; }
            set { SetProp(original.Description, value, v => original.Description = v, "Description"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Payment, UiPayment> {

            internal TheService() : base(XmlDal.DataContext.Payments, new KeyValuePair<string, Type>[] { /*add forward refs here*/ }) { }

            protected override Payment GetOriginal(UiPayment item) { return item.original; }

            protected override UiPayment Create(Payment original) { return new UiPayment(original); }
        }
        #endregion
    }
}
