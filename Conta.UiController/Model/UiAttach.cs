using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiAttach : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service = (service ?? new TheService()); } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Attach original;

        public UiAttach(Attach original)
            : base() {
            this.original = original;
        }

        [StringLength(100)]
        public string Description {
            get { return original.Description; }
            set { SetProp(original.Description, value, v => original.Description = v, "Description"); }
        }

        [StringLength(100)]
        [Required()]
        public string FileName {
            get { return original.FileName; }
            set { SetProp(original.FileName, value, v => original.FileName = v, "FileName"); }
        }

        [Required()]
        public byte[] Contents {
            get { return original.Contents; }
            set { SetProp(original.Contents, value, v => original.Contents = v, "Contents"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Attach, UiAttach> {

            internal TheService() : base(XmlDal.DataContext.Attachs, new KeyValuePair<string, Type>[] { /*add forward refs here*/ }) { }

            protected override Attach GetOriginal(UiAttach item) { return item.original; }

            protected override UiAttach Create(Attach original) { return new UiAttach(original); }
        }
        #endregion
    }
}
