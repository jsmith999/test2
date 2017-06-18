using Conta.DAL.Model;

namespace SqlDal.ServiceHandler {
    class ClientServiceHandler : TableService<Client, int> {
        public ClientServiceHandler() {
            TableName = "clients";
            KeyName = "Id";
        }

        protected override bool DoUpdate(Client item) {
            item.Id += 1;
            return true;
        }
    }
}
