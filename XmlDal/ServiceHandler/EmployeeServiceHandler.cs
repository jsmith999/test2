﻿using System;
using System.Data;
using System.Diagnostics;
using Conta.DAL.Model;

namespace XmlDal.ServiceHandler {
    class EmployeeServiceHandler : TableService<Employee, int> {
        public EmployeeServiceHandler() {
            TableName = "Employee";
            KeyName = "Id";
        }

        public override int GetKeyValue(Employee item) { return item.Id; }

        protected override void DataToModel(Employee item, DataRow row) {
            item.Id = (int)row[0];
            item.Name = row[1] as string;
            item.Surname = row[2] as string;
            item.DOB = (DateTime)row[3];
            item.Salary = (float)row[4];
            item.HireDate = (DateTime)row[5];
            //item.Position = row[4] is DBNull ? 0 : (int)row[4];
        }

        protected override Employee DataToModel(DataRow row) {
            var result = new Employee();
            DataToModel(result, row);
            return result;
        }

        protected override void ModelToData(Employee item, DataRow row) {
            //row[0]=item.Id;
            row[1] = item.Name;
            row[2] = item.Surname;
            row[3] = item.DOB;
            row[4] = item.Salary;
            row[5] = item.HireDate;
        }

        protected override DataRow FindRow(DataTable table, Employee item) {
            var results = table.Select("id = " + item.Id);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }
    }
}
