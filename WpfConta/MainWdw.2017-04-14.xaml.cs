using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Conta.UiController.Controller;
using System.Collections;
using Conta.Model;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Conta.Dal;

namespace WpfConta {
    /// <summary>
    /// Interaction logic for MainWdw.xaml
    /// </summary>
    public partial class MainWdw : Window, IMainView {
        private readonly MainController controller;
        private IList DataSource;
        private DetailGridBuilder detailCtrl;
        public static ObservableCollection<IUiProjectItem> ProjectItems;

        public MainWdw() {
            InitializeComponent();

            theGrid.AutoGeneratingColumn += theGrid_AutoGeneratingColumn;
            controller = new MainController(this);
            theGrid.RowDetailsTemplateSelector = new MyDataTemplateSelector();
        }

        void theGrid_AutoGeneratingColumn(object sender, Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e) {
            var prop = controller.CurrentType.GetProperty(e.PropertyName);
            if (prop == null) return;
            var attrs = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
            if (attrs.Length == 0) return;
            e.Cancel = !(attrs[0] as BrowsableAttribute).Browsable;
        }

        #region IMainView Members
        public object GridDataSource {
            set {
                this.DataContext = value;
                DataSource = value as IList;
                if (this.detailCtrl != null) this.detailCtrl.Dispose();

                this.projectGrid.ItemsSource = DataSource;
                if (DataSource is ObservableCollection<IUiProjectItem>)
                    ProjectItems = DataSource as ObservableCollection<IUiProjectItem>;
                foreach (var item in DataSource) {
                    this.detailCtrl = DetailGridBuilder.Build(this.DetailsGrid, this.DetailBorder, item.GetType(), this.theGrid);
                    return;
                }
            }
        }

        public bool GridReadOnly {
            set { /* built-in by design */ }
        }

        public int DetailDataSourceIndex {
            set { /* no need : detail items bound directly to grid's selection */ }
        }

        public void SetDetail(Type type) {
            /* no need : detail items bound directly to grid's selection */
        }

        public void SetRowStatus(int index, Conta.UiController.Controller.RowStatus status) {
            // built-in by design 
        }

        public event EventHandler<TypeEventArgs> JumpToData;
        #endregion

        private void FileExitMenu_Click(object sender, RoutedEventArgs e) {
            if (controller.CanClose())
                this.Close();
        }

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = !controller.CanClose();
            base.OnClosing(e);
        }

        private void JumpToClient_Click(object sender, RoutedEventArgs e) {
            RaiseJumpTo(typeof(UiClient));
        }

        private void JumpToEmployee_Click(object sender, RoutedEventArgs e) {
            RaiseJumpTo(typeof(UiEmployee));
        }

        private void JumpToProject_Click(object sender, RoutedEventArgs e) {
            RaiseJumpTo(typeof(UiProjectItem));
        }

        private void RaiseJumpTo(Type type) {
            if (JumpToData != null)
                JumpToData(this, new TypeEventArgs(type));
        }

        private void theGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            controller.SelectionChanged(e.AddedItems.Count == 1 ?
                e.AddedItems[0] as UiBase :
                null);
        }

        public void SelectGrid(int gridType) {
            theGrid.Visibility = gridType == 0 ? Visibility.Visible : Visibility.Collapsed;
            projectGrid.Visibility = gridType == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowDetails(object sender, RoutedEventArgs e) {
            UiProjectItem model = (sender as Button).DataContext as UiProjectItem;
            Debug.WriteLine(model.Name);

            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
                Debug.WriteLine(vis.GetType().Name);
                if (vis is Microsoft.Windows.Controls.DataGridRow) {
                    var row = (Microsoft.Windows.Controls.DataGridRow)vis;
                    //row..DataContext = ProjectItems;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
            }
        }

        class MyDataTemplateSelector : DataTemplateSelector {
            public override DataTemplate SelectTemplate(object item, DependencyObject container) {
                return base.SelectTemplate(item, container);
            }
        }
    }

}
