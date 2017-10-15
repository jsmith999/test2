using Conta.UiController.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfConta {
    /// <summary>
    /// Interaction logic for BoSelector.xaml
    /// </summary>
    public partial class BoSelector : Window, IBaseCustomView {
        private EditableListSelectorController controller;

        public BoSelector() {
            InitializeComponent();
            mainGrid.MainGridSelectionChanged += mainGrid_MainGridSelectionChanged;
            DataContextChanged += BoSelector_DataContextChanged;
        }

        private void mainGrid_MainGridSelectionChanged(object sender, Conta.Dal.UiBase e) {
            if (MainGridSelectionChanged != null)
                MainGridSelectionChanged(sender, e);
        }

        void BoSelector_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            controller = e.NewValue as EditableListSelectorController;
            Debug.WriteLine("controller is " + (controller == null ? "" : "not ") + "null");
            //mainGrid.DataContext = controller;
            controller.SelectionChanged(null);
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
            controller.Setter(this.mainGrid.SelectedItem);
            this.Close();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
            controller.Setter(null);
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }

        #region IBaseCustomView
        public string SearchValue { get { return this.mainGrid.SearchValue; } }

        public object SelectedItem {
            get { return this.mainGrid.SelectedItem; }
            set { this.mainGrid.SelectedItem = value; }
        }

        public object GridDataSource { set { this.mainGrid.GridDataSource = value; } }

        public bool GridReadOnly { set { mainGrid.GridReadOnly = value; } }

        public event EventHandler AddBOItem;

        public event EventHandler DelBOItem;

        public event EventHandler StartSearch;

        public event EventHandler<Conta.Dal.UiBase> MainGridSelectionChanged;

        //public event EventHandler ForwardListSelected;

        public MessageActions ShowMessage(string title, string message, MessageActions action) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
