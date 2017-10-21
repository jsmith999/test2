using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using CategoryBudget = Conta.UiController.Model.Reports.ProjectCategorySplit.CategoryBudget;

namespace WpfConta.FlowReports {
    class ProjectDistributionByCategoryChartDataConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var list = value as List<CategoryBudget>;
            if (list == null) return null;

            var result = new SeriesCollection();
            foreach (var data in list)
                result.Add(new PieSeries {
                    Title = data.CategoryName,
                    DataLabels = true,
                    //LabelPoint = null,
                    PushOut = 0d,
                    Values = new ChartValues<double>(new [] { data.Value }),
                });
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
