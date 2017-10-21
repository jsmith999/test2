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

namespace WpfConta.FlowReports {
    /// <summary>
    /// Interaction logic for FlowReport.xaml
    /// </summary>
    public partial class FlowReport : Window {
        public FlowReport() {
            InitializeComponent();

            Title = "To Do";
            ColName1 = "Project Name";
            ColName2 = "Budget";
        }

        public string ColName1 { get; set; }
        public string ColName2 { get; set; }
    }
}
