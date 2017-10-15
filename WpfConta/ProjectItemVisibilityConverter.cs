using Conta.DAL.Model;
using System;
using System.Windows;
using System.Windows.Data;

namespace WpfConta
{
    /* not used
    class ProjectItemVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is ProjectRowType && ((ProjectRowType)value) == ProjectRowType.Category ?
                Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /* */
}
