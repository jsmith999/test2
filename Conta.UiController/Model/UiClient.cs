using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public class UiClient : UiBase {
        #region Service
        private static ClientService service;
        
        public static IDataClientService Service { get { return service; } }
        
        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new ClientService();
        }
        #endregion

        internal readonly Client original;

        public UiClient(Client original) :base() {
            this.original = original;
        }

        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [DisplayName("First Name")]
        [StringLength(100)]
        public string FirstName {
            get { return original.FirstName; }
            set { SetProp(original.FirstName, value, x => original.FirstName = x, "FirstName"); }
        }

        [DisplayName("Last Name")]
        [StringLength(100)]
        public string LastName {
            get { return original.LastName; }
            set { SetProp(original.LastName, value, x => original.LastName = x, "LastName"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class ClientService : BaseUiService<Client, UiClient> {

            internal ClientService() : base(XmlDal.DataContext.Clients, new[] { new KeyValuePair<string, Type>("Poject", typeof(UiProject)) }) { }

            protected override Client GetOriginal(UiClient item) { return item.original; }

            protected override UiClient Create(Client original) { return new UiClient(original); }
        }
        #endregion
    }
}
