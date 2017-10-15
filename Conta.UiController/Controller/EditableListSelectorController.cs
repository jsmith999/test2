using Conta.Dal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conta.UiController.Controller {
    public class EditableListSelectorController : DefaultCustomController {
        private Type targetType;

        public EditableListSelectorController(IBaseCustomView view, Type targetType)
            : base(view) {
            this.targetType = targetType;
            Debug.Assert(targetType != null);
            Debug.Assert(typeof(UiBase).IsAssignableFrom(targetType));

            DataViewSourceChanged(new DataViewParameter(BusinessObject.Declared.FirstOrDefault(x => x.UiDataType == targetType), null));
        }

        public Action<object> Setter { get; set; }
        public void AddItem(UiBase newItem) { base.service.Update(newItem); }
    }
}
