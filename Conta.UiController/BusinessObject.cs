using Conta.Dal;
using Conta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conta.UiController {
    public class BusinessObject {
        public static List<BusinessObject> Declared = new List<BusinessObject> { 
            //new BusinessObject("", typeof()),
            new BusinessObject("Clients", typeof(UiClient)),
            new BusinessObject("Employees", typeof(UiEmployee)),
            new BusinessObject("Projects", typeof(UiProject)),
            new BusinessObject("Materials", typeof(UiMaterial)),
            new BusinessObject("Project Categories", typeof(UiProjectCategory)),
            new BusinessObject("Addresses", typeof(UiAddress)),
            new BusinessObject("Payments", typeof(UiPayment)),
            new BusinessObject("Project Status", typeof(UiProjectStatus)),
        };

        private BusinessObject() { }

        private BusinessObject(string name, Type uiDataType) {
            Name = name;
            UiDataType = uiDataType;
        }

        public string Name { get; private set; }
        public Type UiDataType { get; private set; }
        public object Selection { get; set; }
    }
}
