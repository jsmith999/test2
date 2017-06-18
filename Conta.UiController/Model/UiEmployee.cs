using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public class UiEmployee : UiBase {
        #region Service
        private static EmployeeService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //service.Dispose();
            service = new EmployeeService();
        }
        #endregion

        internal readonly Employee original;

        public UiEmployee(Employee original) {
            this.original = original;
        }

        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [DisplayName("First Name")]
        [StringLength(10)]
        public string FirstName {
            get { return original.FirstName; }
            set { SetProp(original.FirstName, value, x => original.FirstName = x, "FirstName"); }
        }

        [StringLength(10)]
        [DisplayName("Last Name")]
        public string LastName {
            get { return original.LastName; }
            set { SetProp(original.LastName, value, x => original.LastName = x, "LastName"); }
        }

        [DisplayName("Date of Birth")]
        public DateTime DOB {
            get { return original.DOB; }
            set { SetProp(original.DOB, value, x => original.DOB = x, "DOB"); }
        }

        public EmployeePosition Position {
            get { return (EmployeePosition)original.Position; }
            set { SetProp(original.Position, (int)value, x => original.Position = x, "Position"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class EmployeeService : BaseUiService<Employee, UiEmployee> {

            public EmployeeService() : base(XmlDal.DataContext.Employees, new KeyValuePair<string, Type>[] { }) { }

            protected override Employee GetOriginal(UiEmployee item) { return item.original; }

            protected override UiEmployee Create(Employee original) { return new UiEmployee(original); }
        }
        #endregion
    }
}

