using Conta.Dal;
using Conta.Model;
using Conta.UiController.Controller;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfConta {
    /// <summary>
    /// Interaction logic for MainWdw.xaml
    /// </summary>
    public partial class MainWdw : Window, IMainView {
        private readonly MainController controller;
        private DetailGridBuilder detailCtrl;
        public static ObservableCollection<UiProjectItemsCategory> ProjectItems;

        public MainWdw() {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
                Back_Executed, Back_CanExecute));

            theGrid.AutoGeneratingColumn += theGrid_AutoGeneratingColumn;
            controller = new MainController(this);
            DataContext = controller;
            JumpToProject_Click(this, null);
        }

        void theGrid_AutoGeneratingColumn(object sender, Microsoft.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e) {
            //Debug.WriteLine("AutoGeneratingColumn(" + e.PropertyName + ")");
            var prop = controller.CurrentType.GetProperty(e.PropertyName);
            if (prop == null) return;
            var attrs = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
            if (attrs.Length == 0) return;
            e.Cancel = !(attrs[0] as BrowsableAttribute).Browsable;
        }

        #region IMainView Members
        public object GridDataSource {
            set {
                if (this.detailCtrl != null) this.detailCtrl.Dispose();
                foreach (var item in (value as IList)) {
                    this.detailCtrl = DetailGridBuilder.Build(this.DetailsGrid, this.DetailBorder, item.GetType(), this.theGrid);
                    break;
                }

                //theGrid.Columns.Clear();
                // TODO : add IsGlobalSearch property
                theGrid.ItemsSource = value as IList;
                //this.SetValue(Window.DataContextProperty, value);
                //DataSource = value as IList;
            }
        }

        public object GridDetailSource {
            set {
                var isDetail = value != null;
                ProjectItems = isDetail ?
                    value as ObservableCollection<UiProjectItemsCategory> :
                    null;
                projectGrid.Visibility = isDetail ? Visibility.Visible : Visibility.Collapsed;
                this.projectGrid.ItemsSource = ProjectItems;
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
            RaiseJumpTo(typeof(UiProject));
        }

        private void RaiseJumpTo(Type type, UiBase parent = null) {
            controller.SetDataType(type, parent);
        }

        private void Back_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = controller.HasParent; }

        private void Back_Executed(object sender, ExecutedRoutedEventArgs e) { /*!controller.GoBack();*/ }

        private void theGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //foreach (var col in theGrid.Columns)
            //    Debug.WriteLine(col.Header + ":" + (e.AddedItems.Count > 0 ? e.AddedItems[0].GetType().Name : "?"));

            //Debug.WriteLine("call SelectionChanged");
            this.controller.SelectionChanged(e.AddedItems.Count == 1 ?
                e.AddedItems[0] as UiBase :
                null);
            /*
            Debug.WriteLine("wait..." + Thread.CurrentThread.ManagedThreadId);
            try {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { })); // DoEvents
                //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => Debug.WriteLine("DoEvents " + Thread.CurrentThread.ManagedThreadId))); // DoEvents
            } catch (Exception ex) {
                Debug.WriteLine("DoEvents " + ex);
            }
            Debug.WriteLine("done..." + Thread.CurrentThread.ManagedThreadId);
            /* */

            this.detailCtrl.Rearrange();
        }

        private void projectGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (controller.DetailController != null)
                controller.DetailController.SelectionChanged(e.AddedItems.Count == 1 ?
                    e.AddedItems[0] as UiBase :
                    null);
        }

        private void ShowDetails(object sender, RoutedEventArgs e) {
            var model = (sender as Button).DataContext as UiProjectItemsCategory;
            Debug.WriteLine(model.Name);

            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual) {
                Debug.WriteLine(vis.GetType().Name);
                if (vis is Microsoft.Windows.Controls.DataGridRow) {
                    var row = (Microsoft.Windows.Controls.DataGridRow)vis;
                    (sender as Button).Content = row.DetailsVisibility == Visibility.Visible ? "+" : "-";
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
            }
        }

        private void SearchValue_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            //Debug.WriteLine(e.SystemKey + "/" + e.Key);   // None/Return
            if (e.Key == System.Windows.Input.Key.Return ||
                e.Key == System.Windows.Input.Key.Enter)
                if (!string.IsNullOrEmpty(this.SearchValue.Text))
                    controller.Search(this.SearchValue.Text);
        }

        private void ForwardList_Selection_Click(object sender, RoutedEventArgs e) {
            var btn = sender as Button;
            //Debug.WriteLine(btn.Content + " / " + btn.Tag);
            RaiseJumpTo(btn.Tag as Type, theGrid.SelectedItem as UiBase);
        }
    }

    public static class CustomCommands {
        public static readonly RoutedUICommand Back = new RoutedUICommand
                (
                        "_Back",
                        "BackName",
                        typeof(CustomCommands),
                        new InputGestureCollection(){
                            new KeyGesture(Key.B, ModifierKeys.Alt),
                        }
                );

        //Define more commands here, just like the one above
    }
}
