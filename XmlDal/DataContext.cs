using Conta.DAL;
using Conta.DAL.Model;
using XmlDal.ServiceHandler;

namespace XmlDal {
    public static class DataContext {
        static DataContext() {
            Addresss = new AddressServiceHandler();
            // TODO : add attach & others
            Employees = new EmployeeServiceHandler();
            Clients = new ClientServiceHandler();
            ProjectItemsCategory = new ProjectItemsCategoryServiceHandler();
            ProjectItemDetail = new ProjectItemMaterialServiceHandler();
            //ProjectItem = new ProjectItemServiceHandler();
            Projects = new ProjectServiceHandler();
            Materials = new MaterialServiceHandler();
            ProjectStatus = new ProjectStatusServiceHandler();
        }

        public static TableService<Address, int> Addresss { get; private set; }
        public static ITableService<Attach> Attachs { get; private set; }
        public static ITableService<Client> Clients { get; private set; }
        public static ITableService<Employee> Employees { get; private set; }
        public static ITableService<Note> Notes { get; private set; }
        public static ITableService<Payment> Payments { get; private set; }
        public static ITableService<Phones> Phoness { get; private set; }
        public static ITableService<ProjectItemCategory> ProjectItemsCategory { get; private set; }
        public static ITableService<ProjectItemDetailMaterial> ProjectItemDetail { get; private set; }
        //public static ITableService<IUniformProjectGrid> ProjectItem { get; private set; }
        public static ITableService<Project> Projects { get; private set; }
        public static TableService<ProjectStatus,int> ProjectStatus { get; private set; }
        public static ITableService<Material> Materials { get; private set; }
    }
}
