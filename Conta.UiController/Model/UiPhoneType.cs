using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiPhoneType : UiBase {
        #region Service
        private static TheService service;
        
        public static IDataClientService Service { get { return service; } }
        
        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly PhoneType original;

        public UiPhoneType(PhoneType original) :base() {
            this.original = original;
        }

        [StringLength(20)]
        [Required()]
        public string Description {
            get { return original.Description; }
            set { SetProp(original.Description, value, v => original.Description = v, "Description"); }
 }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<PhoneType, UiPhoneType> {

            internal TheService() : base(XmlDal.DataContext.PhoneTypes, new KeyValuePair<string, Type>[]{ /*add forward refs here*/ }) { }

            protected override PhoneType GetOriginal(UiPhoneType item) { return item.original; }

            protected override UiPhoneType Create(PhoneType original) { return new UiPhoneType(original); }
        }
        #endregion
}
}
