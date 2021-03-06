﻿using Conta.Dal;
using Conta.Model;
using Conta.UiController;
using Conta.UiController.Controller;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Conta.UiController.Model.Reports;
using System.Reflection;

namespace WpfConta {
    /// <summary>
    /// Interaction logic for MainWdw.xaml
    /// </summary>
    public partial class MainWdw : Window, IMainView {
        private readonly IDataController controller;
        private IBaseCustomView customControl;
        private IDisposable dataViewSourceSink;
        //private IBaseCustomController customController;

        public MainWdw() {
            InitializeComponent();

            this.Loaded += MainWdw_Loaded;
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
                Back_Executed, Back_CanExecute));

            //theGrid.AutoGeneratingColumn += theGrid_AutoGeneratingColumn;
            controller = new MainController(this);
            DataContext = controller;
            //JumpToProject_Click(this, null);

            dataViewSourceSink = AppServices.Instance.DataViewSource.Register(ChangeMainControl);
            InitMenu();
        }

        void MainWdw_Loaded(object sender, RoutedEventArgs e) {
            this.MainContent.Navigating += MainContent_Navigating;
        }

        void MainContent_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e) {
            //Debug.WriteLine("Navigating to " + e.Uri);
        }

        #region IMainView Members
        public object GridDataSource {
            set {
                if (customControl == null) return;
                Debug.WriteLine("Count:" + (value as IList).Count);
                customControl.GridDataSource = value;
            }
        }

        public object GridDetailSource {
            set {
                var isDetail = value != null;
                var ProjectItems = isDetail ?
                    value as ObservableCollection<UiProjectItemsCategory> :
                    null;
                //projectGrid.Visibility = isDetail ? Visibility.Visible : Visibility.Collapsed;
                ////this.projectGrid.ItemsSource = ProjectItems;
                //this.projectGrid.DataContext = ProjectItems;
            }
        }

        public bool GridReadOnly {
            set { /* built-in by design */ }
        }

        public IUiBase DetailSelection {
            set { /* no need : detail items bound directly to grid's selection */ }
        }

        public void SetDetail(Type type) {
            /* no need : detail items bound directly to grid's selection */
        }

        public void SetRowStatus(int index, RowStatus status) {
            // built-in by design 
        }

        public void SetSelection(IUiBase item) {
            if (customControl != null)
                customControl.SelectedItem = item;
        }

        public void SetDetailSelection(IUiBase item) {
            //var detail = item as UiProjectItemDetail;
            //Debug.Assert(detail != null);
            //foreach (UiProjectItemsCategory category in this.projectGrid.Items)
            //    if (category.Details.Contains(detail)) {
            //        //MainContent.MainGrid.SelectedItem = item;   // TODO : add property
            //        customControl.SelectedItem = item;
            //        break;
            //    }
        }

        public MessageActions ShowMessage(string title, string message, MessageActions action) {
            var msgButton =
                action == (MessageActions.Ok | MessageActions.Cancel) ? MessageBoxButton.OKCancel :
                action == (MessageActions.Yes | MessageActions.No) ? MessageBoxButton.YesNo :
                action == (MessageActions.Yes | MessageActions.No | MessageActions.Cancel) ? MessageBoxButton.YesNoCancel :
                MessageBoxButton.OK;
            var result = MessageBox.Show(message, title, msgButton);
            return result == MessageBoxResult.OK ? MessageActions.Ok :
                result == MessageBoxResult.Yes ? MessageActions.Yes :
                result == MessageBoxResult.No ? MessageActions.No :
                result == MessageBoxResult.Cancel ? MessageActions.Cancel :
                MessageActions.None;
        }

        public void SetReports(IEnumerable<string> headers) {
            foreach (MenuItem menuItem in this.Reports.Items)
                menuItem.Click -= Report_Click;

            this.Reports.Items.Clear();
            if (headers == null) return;

            foreach (var header in headers) {
                var menuItem = new MenuItem {
                    Header = header,
                    IsEnabled = true,
                };
                menuItem.Click += Report_Click;
                Reports.Items.Add(menuItem);
            }
        }

        void Report_Click(object sender, RoutedEventArgs e) {
            // TODO : move the report to the controller
            /*
            Mouse.OverrideCursor = Cursors.Wait;
            // TODO : start animation
            this.controller.ExecuteReport((sender as MenuItem).Header as string);
            /* */
            ShowDemoReport();
        }

        public void ShowReport(string contents) {
            // TODO : add transition & index constants

            var browser = new WebBrowser { Name = "ReportArea" };   // TODO : add report name as suffix
            browser.NavigateToString(contents);
            MainContent.NavigationService.Navigate(browser);
            // TODO : end animation
            Mouse.OverrideCursor = null;
        }
        #endregion

        #region events
        private void FileExitMenu_Click(object sender, RoutedEventArgs e) {
            if (controller.CanClose())
                this.Close();
        }

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = !controller.CanClose();
            if (!e.Cancel) this.dataViewSourceSink.Dispose();
            base.OnClosing(e);
        }

        private void Back_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = controller.HasParent; }

        private void Back_Executed(object sender, ExecutedRoutedEventArgs e) { /*!controller.GoBack();*/ }

        private void projectGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            /* !
                if (controller.DetailController != null)
                    controller.DetailController.SelectionChanged(e.RemovedItems.Count == 1 ?
                        e.RemovedItems[0] as UiBase :
                        null);
            /* */
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

        private void ForwardList_Selection_Click(object sender, RoutedEventArgs e) {
            var btn = sender as Button;
            if (btn == null || btn.Tag == null || !(btn.Tag is BusinessObject)) return;
            //Debug.WriteLine(btn.Content + " / " + btn.Tag);
            //RaiseJumpTo(btn.Tag as Type, MainContent.MainGrid.SelectedItem as UiBase);  
            //RaiseJumpTo(btn.Tag as Type, customControl.SelectedItem as UiBase);  
            AppServices.Instance.DataViewSource.Post(new DataViewParameter(btn.Tag as BusinessObject, customControl.SelectedItem as UiBase));
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e) {
            var dialog = new About();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void CloseReportBtn_Click(object sender, RoutedEventArgs e) {
            // TODO : add transition
            MainContent.NavigationService.GoBack();
        }
        #endregion

        #region jump menu
        private void InitMenu() {
            var lastName = ConfigurationManager.AppSettings["boName"];
            if (string.IsNullOrEmpty(lastName))
                lastName = BusinessObject.Declared[0].Name;

            foreach (var bo in BusinessObject.Declared) {
                var menuItem = new MenuItem {
                    Name = "JumpTo" + bo.Name.Replace(" ", ""),
                    Header = bo.Name,
                };
                menuItem.Click += BusinessObjectMenuItem_Click;
                BusinessObjectsRoot.Items.Add(menuItem);
                if (bo.Name == lastName)
                    BusinessObjectMenuItem_Click(menuItem, null);
            }
        }

        private void BusinessObjectMenuItem_Click(object sender, RoutedEventArgs e) {
            var menuItem = sender as MenuItem;
            var bo = BusinessObject.Declared.FirstOrDefault(b => b.Name == menuItem.Header as string);
            if (bo == null) return;

            // TODO:
            //if (customControl != null)
            //    customControl.Dispose();

            var selection = customControl == null ? null : customControl.SelectedItem as UiBase;
            AppServices.Instance.DataViewSource.Post(new DataViewParameter(bo, null/*selection*/));

            // hack:
            SetReports(bo.Name == "Projects" ? new[] { "Budget" } : new string[0]);
        }

        private void ChangeMainControl(DataViewParameter info) {
            var bo = info.BusinessObject;
            // TODO : use info.Filter

            if (customControl != null) {
                var oldController = customControl.DataContext as IBaseCustomController;
                oldController.CanClose();
                oldController.Save();
            }

            // TODO : generalize this
            object crtControl = bo.UiDataType == typeof(UiProject) ?
                new ProjectGrid() as object :
                new SearchableGrid() as object;
            customControl = crtControl as IBaseCustomView;

            IBaseCustomController customController = null;
            if (bo.UiDataType == typeof(UiProject)) {
                var mdController = new MasterDetailController(crtControl as IDetailCustomView);
                mdController.MainController = new DefaultCustomController((customControl as ProjectGrid).mainGrid);
                customController = mdController as IBaseCustomController;
            } else {
                customController = new DefaultCustomController(crtControl as IBaseCustomView) as IBaseCustomController;
            }
            customControl.DataContext = customController;

            var pageWrap = new Page {
                Title = "[" + info.BusinessObject.UiDataType.Name.Substring(2) + "]",   // this will apear in the navigator's list
                Content = crtControl,
                Name = "pageWrap",
            };
            this.MainContent.NavigationService.Navigate(pageWrap);
        }
        #endregion

        private void ShowDemoReport() {
            if (UiProject.Service == null) UiProject.InitService();
            var reportCtrl = new WpfConta.FlowReports.FlowReport {
                Title = "Project budgets by categories",
                ColName1 = "Project name",
                ColName2 = "Budget",
                DataContext = ProjectCategorySplit.GetData(/* TODO : filter*/),
            };

            var scroller = new ScrollViewer {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = reportCtrl,
            };
            this.MainContent.Content = scroller;
            //reportCtrl.ShowDialog();
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
