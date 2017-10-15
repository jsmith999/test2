using Conta.Dal;
using Conta.Model;
using Conta.UiController;
using Conta.UiController.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfConta {
    /// <summary>
    /// Interaction logic for ProjectGrid.xaml
    /// </summary>
    public partial class ProjectGrid : UserControl, IDetailCustomView {
        private MasterDetailController controller;

        public ProjectGrid() {
            InitializeComponent();

            DataContextChanged += SearchableGrid_DataContextChanged;
            this.mainGrid.MainGridSelectionChanged += mainGrid_MainGridSelectionChanged;
        }

        #region IDetailCustomView
        public IEnumerable GridDetailSource {
            get { return detailGrid.ItemsSource; }
            set { detailGrid.ItemsSource = value; }
        }

        public void DetailSelection(Conta.Dal.IUiBase item) {
            throw new NotImplementedException();
        }
        #endregion

        #region IBaseCustomView
        public string SearchValue {
            get { return mainGrid.SearchValue; }
        }

        public object SelectedItem {
            get { return mainGrid.SelectedItem; }
            set { mainGrid.SelectedItem = value; }
        }

        public object GridDataSource {
            set { mainGrid.GridDataSource = value; }
        }

        public bool GridReadOnly {
            set { mainGrid.GridReadOnly = value; }
        }

        public event EventHandler AddBOItem;

        public event EventHandler DelBOItem;

        public event EventHandler StartSearch;

        public event EventHandler<Conta.Dal.UiBase> MainGridSelectionChanged;

        public event EventHandler ForwardListSelected;

        public MessageActions ShowMessage(string title, string message, MessageActions action) {
            return mainGrid.ShowMessage(title, message, action);
        }
        #endregion

        #region event sinks
        void SearchableGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            controller = e.NewValue as MasterDetailController;
        }

        void mainGrid_MainGridSelectionChanged(object sender, Conta.Dal.UiBase e) {
            controller.SelectionChanged(e);
        }

        private void detailGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            controller.SelectionChanged(e.RemovedItems.Count == 1 ?
                e.RemovedItems[0] as UiBase :
                null);
        }

        private void ShowDetails(object sender, RoutedEventArgs e) {
            var model = (sender as Button).DataContext as UiProjectItemsCategory;
            Debug.WriteLine(model.Name);

            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
                Debug.WriteLine(vis.GetType().Name);
                if (vis is DataGridRow) {
                    var row = (DataGridRow)vis;
                    (sender as Button).Content = row.DetailsVisibility == Visibility.Visible ? "+" : "-";
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
            }
        }
        #endregion

        private void AddChild_Click(object sender, RoutedEventArgs e) {
            var dlg = new BoSelector();
            var childController = new EditableListSelectorController(dlg, typeof(UiMaterial));
            childController.Setter = controller.AddMaterial;
            dlg.DataContext = childController;
            dlg.ShowDialog();
        }

        private void DelChild_Click(object sender, RoutedEventArgs e) {
            if (detailGrid.SelectedItems.Count != 1)
                return;
            Debug.WriteLine(this.detailGrid.SelectedItems[0].ToString());
            //Debug.WriteLine(this.CategoryDetailsGrid.SelectedItems.Count);
            var projectItem = this.detailGrid.SelectedItems[0] as UiProjectItemsCategory;
            var selected = projectItem.SelectedDetail as UiProjectItemDetail;
            if (projectItem == null ||
                projectItem.SelectedDetail == null ||
                selected == null)
                return;
            
            projectItem.Details.Remove(selected);
            UiProjectItemDetail.Service.Delete(selected);
        }
    }
}
