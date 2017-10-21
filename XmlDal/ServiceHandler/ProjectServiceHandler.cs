using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Conta.DAL.Model;

namespace XmlDal.ServiceHandler {
    class ProjectServiceHandler : TableService<Project, int> {
        private ProjectStatusServiceHandler projectStatusServiceHandler;
        private AddressServiceHandler addressServiceHandler;
        private ClientServiceHandler clientServiceHandler;

        public ProjectServiceHandler() {
            TableName = "Project";
            KeyName = "Id";

            projectStatusServiceHandler = new ProjectStatusServiceHandler();
            addressServiceHandler = new AddressServiceHandler();
            clientServiceHandler = new ClientServiceHandler();
        }

        public override int GetKeyValue(Project item) { return item.Id; }

        protected override DataRow FindRow(DataTable table, Project item) {
            var results = table.Select("Id = " + item.Id);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }

        protected override void DataToModel(Project item, DataRow row) {
            item.Id = (int)row[0];
            item.Name = (string)row[1];
            item.Price = (float)row[2];
            item.StartDate = (DateTime)row[3];
            item.EndDate = (DateTime)row[4];
            item.StatusKey = (int)row[5];
            item.Status = projectStatusServiceHandler.FromKey(item.StatusKey);
            item.AddressKey = (int)row[6];
            if (item.AddressKey != 0)
                item.Address = addressServiceHandler.FromKey(item.AddressKey);
            item.Receivable = (string)row[7];
            item.AgeOfReceivable = (string)row[8];
            item.ClientId = (int)row[9];
            //if (item.ClientKey != 0)
            //    item.Client = clientServiceHandler.FromKey(item.ClientKey);
        }

        protected override Project DataToModel(DataRow row) {
            var result = new Project();
            DataToModel(result, row);
            return result;
        }

        protected override void ModelToData(Project item, DataRow row) {
            //row[0]=item.Id;
            row[1] = item.Name;
            row[2] = item.Price;
            row[3] = item.StartDate;
            row[4] = item.EndDate;
            row[5] = item.StatusKey;
            row[6] = item.AddressKey;
            row[7] = item.Receivable;
            row[8] = item.AgeOfReceivable;
            row[9] = item.ClientId;
        }

        protected override Project DoCreate() {
            var result = base.DoCreate();
            
            // defaults
            result.StatusKey = 0;
            result.AddressKey = 0;
            result.ClientId = 0;

            return result;
        }
    }
}
