using System.Data;
using System.Diagnostics;
using Conta.DAL.Model;

namespace XmlDal.ServiceHandler {
    partial class ClientServiceHandler : TableService<Client, int> {
        public ClientServiceHandler() {
            TableName = "client";
            KeyName = "Id";
        }

        protected override bool DoUpdate(Client item) {
            item.Id += 1;
            return true;
        }

        //protected override Dal.Client DoDelete(Dal.Client item) { return item; }

        public override int GetKeyValue(Client item) { return item.Id; }

        protected override void DataToModel(Client item, DataRow row) {
            item.Id = (int)row[0];
            item.FirstName = row[1] as string;
            item.LastName = row[2] as string;
        }

        protected override Client DataToModel(DataRow row) {
            var result = new Client();
            DataToModel(result, row);
            return result;
        }

        protected override void ModelToData(Client item, DataRow row) {
            //row[0]=item.Id;
            row[1] = item.FirstName;
            row[2] = item.LastName;
        }

        protected override System.Data.DataRow FindRow(System.Data.DataTable table, Client item) {
            var results = table.Select("id = " + item.Id);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }

        protected override Client DoCreate() {
            throw new System.NotImplementedException();
        }
    }
}
