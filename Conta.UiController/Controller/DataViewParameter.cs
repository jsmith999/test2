using System;
using Conta.Dal;

namespace Conta.UiController.Controller {
    public class DataViewParameter {
        public DataViewParameter(BusinessObject businessObject, UiBase filter) {
            if (businessObject == null)
                throw new ArgumentNullException("businessObject");

            BusinessObject = businessObject;
            Filter = filter;
        }

        public BusinessObject BusinessObject { get; private set; }
        public UiBase Filter{ get; private set; }
    }
}
