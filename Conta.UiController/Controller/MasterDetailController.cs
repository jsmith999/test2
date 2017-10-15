using Conta.Dal;
using Conta.DAL;
using Conta.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public override void SelectionChanged(Dal.IUiBase item) {
            if (item is UiProject) {
                // master
                base.SelectionChanged(item);
                (view as IDetailCustomView).GridDetailSource = item == null ?
                    null :
                    detailService.GetList(item);
            } else {
                // detail
                var detailItem = item as UiProjectItemsCategory;    // could be material
                if (detailItem != null && detailItem.IsDirty)
                    UiProjectItemsCategory.Service.Update(detailItem);
            }
        }

        public void AddMaterial(object inserted) {
            var newMaterial = inserted as UiMaterial;
            UiProjectItemDetail itemDetail = UiProjectItemDetail.Service.Create() as UiProjectItemDetail;
            var project = view.SelectedItem as UiProject;
            itemDetail.original.Project = project.Id;
            itemDetail.original.Category = newMaterial.Category;
            foreach (UiProjectItemsCategory category in detailService.GetList(view.SelectedItem as UiProject))
                if (category.Key == newMaterial.Category) {
                    itemDetail.Material = newMaterial;
                    category.Details.Add(itemDetail);
                    UiProjectItemDetail.Service.Update(itemDetail);
                    return;     // TODO : refresh
                }
        }
    }
}
