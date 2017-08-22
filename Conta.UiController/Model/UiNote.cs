using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
    public partial class UiNote : UiBase {
        #region Service
        private static TheService service;

        public static IDataClientService Service { get { return service; } }

        public static void InitService() {
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }
        #endregion

        internal readonly Note original;

        public UiNote(Note original)
            : base() {
            this.original = original;
        }

        [Required()]
        [Browsable(false)]
        public int Id { get { return original.Id; } }

        [StringLength(100)]
        [Required()]
        public string Description {
            get { return original.Description; }
            set { SetProp(original.Description, value, v => original.Description = v, "Description"); }
        }

        [StringLength(1000000)]
        [Required()]
        public string Contents {
            get { return original.Contents; }
            set { SetProp(original.Contents, value, v => original.Contents = v, "Contents"); }
        }

        public override IDataClientService GetService() { return Service; }

        #region service implementation
        class TheService : BaseUiService<Note, UiNote> {

            internal TheService() : base(XmlDal.DataContext.Notes, new KeyValuePair<string, Type>[] { /*add forward refs here*/ }) { }

            protected override Note GetOriginal(UiNote item) { return item.original; }

            protected override UiNote Create(Note original) { return new UiNote(original); }
        }
        #endregion
    }
}
