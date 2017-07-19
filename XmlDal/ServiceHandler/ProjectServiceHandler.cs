using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XmlDal.ServiceHandler {
    class ProjectServiceHandler : TableService<Project, int> {
        public const string TheTableName = "Project";

        internal ProjectServiceHandler()
            : base() {
            TableName = TheTableName;
            KeyName = "Key";
        }

        public override int GetKeyValue(Project item) { return item.Id; }

        protected override void DataToModel(Project item, DataRow row) {
            item.Id = (int)row[0];
            item.Name = row[1] as string;
            item.Budget = (double)row[2];
            item.ClientKey = (int)row[3];
        }

        protected override Project DataToModel(DataRow row) {
            var result = new Project();
            DataToModel(result, row);
            return result;
        }

        protected override void ModelToData(Project item, DataRow row) {
            //row[0]=item.Id;
            row[1] = item.Name;
            row[2] = item.Budget;
            row[3] = item.ClientKey;
        }

        protected override System.Data.DataRow FindRow(System.Data.DataTable table, Project item) {
            var results = table.Select("id = " + item.Id);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }
    }
}
