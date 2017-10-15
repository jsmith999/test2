using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiClient : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Client original;

        public UiClient(Client original)
            : base() {
            this.original = original;
        }

        [Required()]
        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [StringLength(100)]
        [Required()]
        public string Name {
            get { return original.Name; }
            set { SetProp(original.Name, value, v => original.Name = v, "Name"); }
        }

        [StringLength(100)]
        [Required()]
        public string Surname {
            get { return original.Surname; }
            set { SetProp(original.Surname, value, v => original.Surname = v, "Surname"); }
        }

        //![Required()]
        [Browsable(false)]
        public int AddressId {
            get {
                return original.AddressId;
            }

            set {
                if (SetProp(original.AddressId, value, v => original.AddressId = v, "AddressId"))
                    Address = value == 0 ?
                        null :
                        XmlDal.DataContext.Addresss.FromKey(value);
            }
        }

        public Address Address {
            get { return original.Address; }
            set { SetProp(original.Address, value, v => { original.Address = v; original.AddressId = v.Id; }, "Address"); }
        }

        [StringLength(100)]
        [Required()]
        public string Email {
            get { return original.Email; }
            set { SetProp(original.Email, value, v => original.Email = v, "Email"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Client, UiClient> {

            internal TheService() : base(XmlDal.DataContext.Clients, new KeyValuePair<string, Type>[] { new KeyValuePair<string, Type>("Projects", typeof(UiProject)) }) { }

            protected override Client GetOriginal(UiClient item) { return item.original; }

            protected override UiClient Create(Client original) { return new UiClient(original); }
        }
        #endregion
    }
}
