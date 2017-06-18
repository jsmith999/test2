using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conta.DAL.ServiceHandler {
    class ClientServiceHandler : TableService<Client, int> {
        public ClientServiceHandler() {
            TableName = "clients";
            KeyName = "Id";
        }

        protected override Client DoUpdate(Client item) {
            item.Id += 1;
            return item;
        }

        //protected override Dal.Client DoDelete(Dal.Client item) { return item; }

        protected override int GetKeyValue(Client item) { return item.Id; }
    }
}
