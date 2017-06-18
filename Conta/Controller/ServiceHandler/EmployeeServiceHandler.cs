using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conta.DAL.Model;

namespace Conta.DAL.ServiceHandler {
    class EmployeeServiceHandler : TableService<Employee, int> {
        public EmployeeServiceHandler() {
            TableName = "Employees";
            KeyName = "Id";
        }

        protected override int GetKeyValue(Employee item) { return item.Id; }
    }
}
