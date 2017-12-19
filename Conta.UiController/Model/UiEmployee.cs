using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiEmployee : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service = (service ?? new TheService()); } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Employee original;

        public UiEmployee(Employee original)
            : base() {
            this.original = original;
        }
        
        [Required()]
        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [StringLength(100)]
        [Required()]
        public string Name {
            get { return original.Name; }
            set { SetProp(original.Name, value, v => original.Name = v, "Name"); }
        }

        [StringLength(20)]
        [Required()]
        [System.ComponentModel.DisplayName("Surname")]
        public string Surname {
            get { return original.Surname; }
            set { SetProp(original.Surname, value, v => original.Surname = v, "Surname"); }
        }

        [Required()]
        [System.ComponentModel.DisplayName("Date Of Birth")]
        public DateTime DOB {
            get { return original.DOB; }
            set { SetProp(original.DOB, value, v => original.DOB = v, "DOB"); }
        }

        [Required()]
        public float Salary {
            get { return original.Salary; }
            set { SetProp(original.Salary, value, v => original.Salary = v, "Salary"); }
        }

        [Required()]
        [System.ComponentModel.DisplayName("Hire Date")]
        public DateTime HireDate {
            get { return original.HireDate; }
            set { SetProp(original.HireDate, value, v => original.HireDate = v, "HireDate"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Employee, UiEmployee> {

            internal TheService() : base(XmlDal.DataContext.Employees, new KeyValuePair<string, Type>[] { /*add forward refs here*/ }) { }

            protected override Employee GetOriginal(UiEmployee item) { return item.original; }

            protected override UiEmployee Create(Employee original) { return new UiEmployee(original); }
        }
        #endregion
    }
}
