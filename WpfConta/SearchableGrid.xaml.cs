#define ShowDetailCtrl
using Conta.Dal;
using Conta.Model;
using Conta.UiController;
using Conta.UiController.Controller;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfConta {
    /// <summary>
    /// Interaction logic for SearchableGrid.xaml
    /// </summary>
    public partial class SearchableGrid : UserControl, IBaseCustomView, INotifyPropertyChanged {
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
            "SelectedItem", typeof(object),
            typeof(SearchableGrid),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, SelectedItemPropertyChanged));

        public static readonly DependencyProperty SelectionCountProperty =
            DependencyProperty.Register(
            "SelectionCount", typeof(int),
            typeof(SearchableGrid),
            new PropertyMetadata(0, SelectionCountPropertyChanged));

        private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            //d.SetValue(e.Property,e.NewValue);
            //typeof(SearchableGrid).GetProperty(e.Property.Name).SetValue(d,e.NewValue);
            (d as SearchableGrid).SelectedItem = e.NewValue;
        }

        private static void SelectionCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            //(d as SearchableGrid).SelectedRows = (int)e.NewValue;
        }

        private IBaseCustomController controller;
        private string timestamp;
#if(ShowDetailCtrl)
        private DetailGridBuilder detailCtrl;   // TODO
#endif

        public SearchableGrid() {
            InitializeComponent();
            timestamp = DateTime.Now.ToString();
            DataContextChanged += SearchableGrid_DataContextChanged;
        }

        void SearchableGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            controller = e.NewValue as IBaseCustomController;
            //RaisePropertyChangedEvent("ForwardLinks");    // NO
            // too early
            //if (controller != null) controller.RaisePropertyChanged("ForwardLinks");
            //Debug.WriteLine(controller.ForwardLinks.Count());
        }

        #region IBaseCustomView
        public DataGrid MainGrid { get { return this.theGrid; } }

        public string SearchValue { get { return this.SearchTextBox.Text; } }

        public object SelectedItem {
            get { return this.theGrid.SelectedItem; }
            set {
                this.theGrid.SelectedItem = value;
                RaisePropertyChangedEvent("SelectedItem");
                RaisePropertyChangedEvent("SelectedRows");
            }
        }

        public int SelectedRows {
            get { return this.theGrid.SelectedItems.Count; }
            //set {
            //    this.selectedRows = value;
            //    RaisePropertyChangedEvent("SelectedRows");
            //}
        }

        public object GridDataSource {
            set {
#if(ShowDetailCtrl)
                if (this.detailCtrl != null) this.detailCtrl.Dispose();
                foreach (var item in (value as IList)) {
                    this.detailCtrl = DetailGridBuilder.Build(this.DetailsGrid, null/*this.DetailBorder*/, item.GetType(), this.theGrid);
                    break;
                }
#endif

                this.theGrid.DataContext = value;
            }
        }

        public bool GridReadOnly { set { this.theGrid.IsReadOnly = value; } }

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

        public event EventHandler AddBOItem;
        public event EventHandler DelBOItem;
        public event EventHandler StartSearch;
        public event EventHandler<UiBase> MainGridSelectionChanged;
        //public event EventHandler ForwardListSelected;
        #endregion

        private void AddItem_Click(object sender, RoutedEventArgs e) { if (AddBOItem != null) AddBOItem(this, EventArgs.Empty); }
        private void DelItem_Click(object sender, RoutedEventArgs e) { if (DelBOItem != null) DelBOItem(this, EventArgs.Empty); }
        private void SearchTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            //Debug.WriteLine(e.SystemKey + "/" + e.Key);   // None/Return
            if (e.Key == System.Windows.Input.Key.Return ||
                e.Key == System.Windows.Input.Key.Enter)
                //if (!string.IsNullOrEmpty(this.SearchValue.Text))
                //controller.Search(this.MainContent.SearchValue);
                if (StartSearch != null)
                    StartSearch(this, e);
        }

        private void theGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selection = e.AddedItems.Count == 1 ?
                e.AddedItems[0] as UiBase :
                null;

            //this.SelectedItem = selection;

            if (MainGridSelectionChanged != null)
                MainGridSelectionChanged(this, selection);

#if(ShowDetailCtrl)
            this.detailCtrl.Rearrange(DetailsGrid.ActualWidth);
#endif
        }

        //private void ForwardList_Selection_Click(object sender, RoutedEventArgs e) { if (ForwardListSelected != null) ForwardListSelected((e.OriginalSource as Button).Tag as Type, EventArgs.Empty); }
        private void ForwardList_Selection_Click(object sender, RoutedEventArgs e) {
            var newType = (e.OriginalSource as Button).Tag as Type;
            var bo = BusinessObject.Declared.FirstOrDefault(b => b.UiDataType == newType);
            if (bo == null) return;

            AppServices.Instance.DataViewSource.Post(new DataViewParameter(bo, SelectedItem as UiBase));
        }

        void theGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            //Debug.WriteLine("AutoGeneratingColumn(" + e.PropertyName + ")");
            var prop = controller.CurrentType.GetProperty(e.PropertyName);
            if (prop == null) return;

            var attrs = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
            if (attrs.Length != 0)
                e.Cancel = !(attrs[0] as BrowsableAttribute).Browsable;
            if (e.Cancel) return;

            attrs = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (attrs.Length != 0)
                e.Column.Header = (attrs[0] as DisplayNameAttribute).DisplayName;

            //DataGridComboBoxColumn
            if (e.Column is DataGridBoundColumn) {
                var column = e.Column as DataGridBoundColumn;
                // combobox
                attrs = prop.GetCustomAttributes(typeof(LookupBoundPropertyAttribute), true);
                if (attrs.Length != 0) {
                    var lookupAttribute = attrs[0] as LookupBoundPropertyAttribute;
                    //         <ComboBox Name="combo" DisplayMemberPath="value" SelectedValuePath="key" SelectedValue="{Binding combo}" ItemsSource="{Binding items}"/>
                    var lookupColumn = new DataGridComboBoxColumn {
                        Header = column.Header,
                        DisplayMemberPath = lookupAttribute.DisplayMember,
                        //IsAutoGenerated = true,
                        SelectedValuePath = lookupAttribute.LookupMember,
                        SelectedValueBinding = GetBinding(e.PropertyName),
                    };
                    //lookupColumn.ItemsSource = GetService(lookupAttribute.DataSource);
                    if (UiProjectStatus.Service == null) UiProjectStatus.InitService();
                    lookupColumn.ItemsSource = UiProjectStatus.Service.GetList();

                    e.Column = lookupColumn;
                    return;
                }

                // date / date/time / time
                if (e.PropertyType == typeof(DateTime)) {
                    // datepicker
                    var dateBinder = GetBinding(e.PropertyName);
                    var factoryElement = new FrameworkElementFactory(typeof(DatePicker));
                    factoryElement.SetValue(DatePicker.DisplayDateProperty, dateBinder);
                    factoryElement.SetValue(DatePicker.SelectedDateProperty, dateBinder);
                    var cellTemplate = new DataTemplate {
                        VisualTree = factoryElement,
                    };
                    var dateColumn = new DataGridTemplateColumn {
                        Header = e.Column.Header,
                        CellTemplate = cellTemplate,
                    };
                    //dateColumn.Binding = GetBinding(e.PropertyName);
                    e.Column = dateColumn;
                    return;
                }

                // all others
                column.Binding = GetBinding(e.PropertyName);
            }
        }

        private Binding GetBinding(string propertyName) {
            return new Binding(propertyName) {
                NotifyOnValidationError = true,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                ValidatesOnDataErrors = true,
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChangedEvent(string propName) {
            if (PropertyChanged == null) return;

            Debug.WriteLine("changed " + propName);
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
