using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDal;
using Conta.DAL.Model;

namespace SqlDal.ServiceHandler {
    class EmployeeServiceHandler : TableService<Employee, int> {
        public EmployeeServiceHandler() {
            TableName = "Employees";
            KeyName = "Id";
        }

        public override int GetKeyValue(Employee item) { return item.Id; }
    }
}
