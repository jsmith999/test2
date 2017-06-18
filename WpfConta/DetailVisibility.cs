using Conta.DAL.Model;
using Conta.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfConta
{
    public class DetailVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = value as IUiProjectItem;
            if (item == null) return false;

            if (item.RowType == ProjectRowType.Category)
                return true;

            var parent = MainWdw.ProjectItems.FirstOrDefault(x => x.Parent == item.Parent);
            return !(parent != null && parent.HasDetails);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
