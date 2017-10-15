using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiPhones : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Phones original;

        public UiPhones(Phones original)
            : base() {
            this.original = original;
        }

        //![Required()]
        [Browsable(false)]
        public int ClientId {
            get { return original.ClientId; }
            set { SetProp(original.ClientId, value, v => original.ClientId = v, "ClientId"); }
        }

        // TODO : use this ?
        //public Client Client { get; set; }

        [StringLength(20)]
        [Required()]
        [System.ComponentModel.DisplayName("Phone Number")]
        public string PhoneNo {
            get { return original.PhoneNo; }
            set { SetProp(original.PhoneNo, value, v => original.PhoneNo = v, "PhoneNo"); }
        }

        [StringLength(10)]
        [System.ComponentModel.DisplayName("Extension")]
        public string Extension {
            get { return original.Extension; }
            set { SetProp(original.Extension, value, v => original.Extension = v, "Extension"); }
        }

        [Required()]
        public int PhoneType {
            get { return original.PhoneType; }
            set { SetProp(original.PhoneType, value, v => original.PhoneType = v, "PhoneType"); }
        }

        [Required()]
        public bool IsMain {
            get { return original.IsMain; }
            set { SetProp(original.IsMain, value, v => original.IsMain = v, "IsMain"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Phones, UiPhones> {

            internal TheService() : base(XmlDal.DataContext.Phoness, new KeyValuePair<string, Type>[] { /*add forward refs here*/ }) { }

            protected override Phones GetOriginal(UiPhones item) { return item.original; }

            protected override UiPhones Create(Phones original) { return new UiPhones(original); }
        }
        #endregion
    }
}
