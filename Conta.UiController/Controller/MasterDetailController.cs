using Conta.DAL;
using Conta.Model;
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace Conta.UiController.Controller {
    public class MasterDetailController : DefaultCustomController {
        protected IDataClientService detailService; // detail service
        private object popupController;

        public MasterDetailController(IDetailCustomView view)
            : base(view) {
            UiProjectItemsCategory.InitService();
            detailService = UiProjectItemsCategory.Service;
        }

        public object PopupController {
            get { return popupController; }
            set { SetValue(ref popupController, value, "PopupController"); }
        }

        public IBaseCustomController MainController { get; set; }

        // TODO: test 
        protected override void SelectionChanged(Dal.IUiBase item) {
            if (item is UiProject) {
                // master
                base.SelectionChanged(item);
                (view as IDetailCustomView).GridDetailSource = item == null ?
                    null :
                    detailService.GetList(item);
            } else {
                // detail
                if (item == null) return;
                if (item is UiProjectItemsCategory)
                    CategoryUpdate(item as UiProjectItemsCategory);
                else
                    if (item.IsDirty)
                        item.Update();
            }
        }

        public override void Save() {
            foreach (UiProject project in base.dataSource) {
                if (project.IsDirty)
                    project.Update();

                foreach (UiProjectItemsCategory item in detailService.GetList(project)) 
                    CategoryUpdate(item);
            }
        }

        private static void CategoryUpdate(UiProjectItemsCategory item) {
            if (item.IsDirty)
                item.Update();

            foreach (var detail in item.Details)
                if (detail.IsDirty)
                    detail.Update();
        }
        protected override void view_AddBOItem(object sender, EventArgs e) { this.AddMaterial(sender); }

        private void AddMaterial(object inserted) {
            var newMaterial = inserted as UiMaterial;
            UiProjectItemDetail itemDetail = UiProjectItemDetail.Service.Create() as UiProjectItemDetail;
            var project = view.SelectedItem as UiProject;
            itemDetail.original.Project = project.Id;
            itemDetail.original.Category = newMaterial.Category;
            foreach (UiProjectItemsCategory category in detailService.GetList(view.SelectedItem as UiProject))
                if (category.Key == newMaterial.Category) {
                    itemDetail.Material = newMaterial;
                    //UiProjectItemDetail.Service.Update(itemDetail);
                    itemDetail.Update();
                    return;
                }
        }
    }
}
