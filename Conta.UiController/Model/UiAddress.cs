using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiAddress : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Address original;

        public UiAddress(Address original)
            : base() {
            this.original = original;
        }
        
        [Required()]
        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [StringLength(20)]
        [Required()]
        [System.ComponentModel.DisplayName("Street No")]
        public string StreetNo {
            get { return original.StreetNo; }
            set { SetProp(original.StreetNo, value, v => original.StreetNo = v, "StreetNo"); }
        }

        [StringLength(100)]
        [Required()]
        [System.ComponentModel.DisplayName("Street line1")]
        public string Street1 {
            get { return original.Street1; }
            set { SetProp(original.Street1, value, v => original.Street1 = v, "Street1"); }
        }

        [StringLength(100)]
        [System.ComponentModel.DisplayName("Street line2")]
        public string Street2 {
            get { return original.Street2; }
            set { SetProp(original.Street2, value, v => original.Street2 = v, "Street2"); }
        }

        [StringLength(100)]
        [Required()]
        public string City {
            get { return original.City; }
            set { SetProp(original.City, value, v => original.City = v, "City"); }
        }

        [StringLength(5)]
        [Required()]
        [System.ComponentModel.DisplayName("Province / State")]
        public string Province {
            get { return original.Province; }
            set { SetProp(original.Province, value, v => original.Province = v, "Province"); }
        }

        [StringLength(20)]
        public string Country {
            get { return original.Country; }
            set { SetProp(original.Country, value, v => original.Country = v, "Country"); }
        }

        [StringLength(10)]
        [System.ComponentModel.DisplayName("Postal Code / Zip")]
        public string PostalCode {
            get { return original.PostalCode; }
            set { SetProp(original.PostalCode, value, v => original.PostalCode = v, "PostalCode"); }
        }

        [StringLength(100)]
        public string Description {
            get { return original.Description; }
            set { SetProp(original.Description, value, v => original.Description = v, "Description"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Address, UiAddress> {

            internal TheService() : base(XmlDal.DataContext.Addresss, new KeyValuePair<string, Type>[] { /*add forward refs here*/ }) { }

            protected override Address GetOriginal(UiAddress item) { return item.original; }

            protected override UiAddress Create(Address original) { return new UiAddress(original); }
        }
        #endregion
    }
}
