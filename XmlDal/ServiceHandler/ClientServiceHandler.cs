using Conta.DAL.Model;
using System.Data;
using System.Diagnostics;

namespace XmlDal.ServiceHandler {
    class ClientServiceHandler : TableService<Client, int> {
        AddressServiceHandler addressServiceHandler;

        public ClientServiceHandler() {
            TableName = "Client";
            KeyName = "Id";

            addressServiceHandler = new AddressServiceHandler();
        }

        public override int GetKeyValue(Client item) { return item.Id; }

        protected override DataRow FindRow(DataTable table, Client item) {
            var results = table.Select("Id = " + item.Id);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }

        protected override void DataToModel(Client item, DataRow row) {
            item.Id = (int)row[0];
            item.Name = (string)row[1];
            item.Surname = (string)row[2];
            item.AddressId = (int)row[3];

            //item.Address = addressServiceHandler.GetList().First();     // TODO : apply filter
            if (item.AddressId != 0) 
                item.Address = addressServiceHandler.FromKey(item.AddressId);

            item.Email = (string)row[4];
        }

        protected override Client DataToModel(DataRow row) {
            var result = new Client();
            DataToModel(result, row);
            return result;
        }

        protected override void ModelToData(Client item, DataRow row) {
            //row[0]=item.Id;
            row[1] = item.Name;
            row[2] = item.Surname;
            row[3] = item.AddressId;
            row[4] = item.Email;
        }
    }
}
