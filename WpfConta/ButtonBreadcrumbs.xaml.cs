using Conta.Dal;
using Conta.UiController;
using Conta.UiController.Controller;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfConta {
    /// <summary>
    /// Interaction logic for ButtonBreadcrumbs.xaml
    /// </summary>
    public partial class ButtonBreadcrumbs : UserControl {
        public ButtonBreadcrumbs() {
            InitializeComponent();
            AppServices.Instance.DataViewSource.Register(AddBreadcrumb);
        }

        private void AddBreadcrumb(DataViewParameter info) {
            var newButton = new Button {
                Content = info.BusinessObject.Name,
                Margin = new Thickness(2d),
                Tag = info,
                ToolTip = info.Filter == null ? string.Empty : info.Filter.ToString(),
            };
            newButton.Click += Breadcrumb_Click;

            this.mainPanel.Children.Add(newButton);
        }

        void Breadcrumb_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if (button == null) return;

            var param = button.Tag as DataViewParameter;
            if (param == null) return;

            //RaiseJumpTo(bo.UiDataType, bo.Selection as UiBase);
            AppServices.Instance.DataViewSource.Post(param);

            while (this.mainPanel.Children.Count > 0) {
                var crumb = this.mainPanel.Children[this.mainPanel.Children.Count - 1] as Button;
                this.mainPanel.Children.RemoveAt(this.mainPanel.Children.Count - 1);
                // TODO : pop sound & delay(200)
                if (crumb == button) break;
            }
        }
    }
}
