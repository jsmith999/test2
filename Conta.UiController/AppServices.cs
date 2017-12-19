using Conta.DAL;
using Conta.UiController.Controller;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Conta.UiController {
    public class AppServices {
        public static AppServices Instance = new AppServices();

        private AppServices() {
            DataViewSource = new ObservableService<DataViewParameter>();
        }

        public ObservableService<DataViewParameter> DataViewSource { get; private set; }

        public IDataClientService GetDataService(Type type) {
            var prop = type.GetProperty("Service", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static);
            if (prop == null) {
                Debugger.Break();   // investigate why
                return null;
            }

            var service = prop.GetValue(null, null);
            Debug.Assert(service != null);  // TODO : throw an exception
            return service as IDataClientService;
        }
    }
}
