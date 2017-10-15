using System;
using System.Data;
using System.Diagnostics;
using Conta.DAL.Model;

namespace XmlDal.ServiceHandler {
    public class AddressServiceHandler : TableService<Address, int> {
        public AddressServiceHandler() {
            TableName = "Address";
            KeyName = "Id";
        }

        public override int GetKeyValue(Address item) { return item.Id; }

        protected override DataRow FindRow(DataTable table, Address item) {
            var results = table.Select("Id = " + item.Id);
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }

        protected override void DataToModel(Address item, DataRow row) {
            item.Id = (int)row[0];
            item.StreetNo = (string)row[1];
            item.Street1 = (string)row[2];
            item.Street2 = (string)row[3];
            item.City = (string)row[4];
            item.Province = (string)row[5];
            item.Country = (string)row[6];
            item.PostalCode = (string)row[7];
            item.Description = (string)row[8];
        }

        protected override Address DataToModel(DataRow row) {
            var result = new Address();
            DataToModel(result, row);
            return result;
        }

        protected override void ModelToData(Address item, DataRow row) {
            //row[0]=item.Id;
            row[1] = item.StreetNo;
            row[2] = item.Street1;
            row[3] = item.Street2;
            row[4] = item.City;
            row[5] = item.Province;
            row[6] = item.Country;
            row[7] = item.PostalCode;
            row[8] = item.Description;
        }
    }
}
