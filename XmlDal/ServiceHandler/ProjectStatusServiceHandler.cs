using System;
using System.Data;
using System.Diagnostics;
using Conta.DAL.Model;

namespace XmlDal.ServiceHandler {
    class ProjectStatusServiceHandler : TableService<ProjectStatus, int> {
        public ProjectStatusServiceHandler() {
            TableName = "ProjectStatus";
            KeyName = "Id";
        }

        public override int GetKeyValue(ProjectStatus item) { return item.Id; }

        protected override DataRow FindRow(DataTable table, ProjectStatus item) {
            var results = table.Select("Id = " + item.Id);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }

        protected override void DataToModel(ProjectStatus item, DataRow row) {
            item.Id = (int)row[0];
            item.Description = (string)row[1];
        }

        protected override ProjectStatus DataToModel(DataRow row) {
            var result = new ProjectStatus();
            DataToModel(result, row);
            return result;
        }

        protected override void ModelToData(ProjectStatus item, DataRow row) {
            //row[0]=item.Id;
            row[1] = item.Description;
        }
    }
}
