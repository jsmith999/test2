using Conta.DAL.Model;
using System;
using System.Windows.Data;

namespace WpfConta
{
    class NotCategoryRow : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(value is ProjectRowType) || ((ProjectRowType)value) != ProjectRowType.Category;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
