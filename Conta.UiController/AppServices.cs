using Conta.Dal;
using Conta.DAL;
using Conta.Model;
using Conta.UiController.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conta.UiController {
    public class AppServices {
        public static AppServices Instance = new AppServices();

        private AppServices() {
            DataViewSource = new ObservableService<DataViewParameter>();
        }

        public ObservableService<DataViewParameter> DataViewSource { get; private set; }

        public IDataClientService GetDataService(Type type) {
            if (type == typeof(UiClient)) {
                UiClient.InitService();
                return UiClient.Service;
            }

            if (type == typeof(UiEmployee)) {
                UiEmployee.InitService();
                return UiEmployee.Service;
            }

            if (type == typeof(UiProjectItemsCategory)) {
                // only as DetailController
                UiProjectItemsCategory.InitService();
                return UiProjectItemsCategory.Service;
            }

            if (type == typeof(UiProject)) {
                UiProject.InitService();
                return UiProject.Service;
            }

            if (type == typeof(UiMaterial)) {
                UiMaterial.InitService();
                return UiMaterial.Service;
            }

            if (type == typeof(UiProjectCategory)) {
                UiProjectCategory.InitService();
                return UiProjectCategory.Service;
            }

            if (type == typeof(UiAddress)) {
                UiAddress.InitService();
                return UiAddress.Service;
            }

            if (type == typeof(UiProjectStatus)) {
                UiProjectStatus.InitService();
                return UiProjectStatus.Service;
            }

            // TODO : throw an exception
            return null;
        }

    }
}
