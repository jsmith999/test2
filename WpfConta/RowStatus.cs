using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Conta.Dal;
using Conta.Model;
using Conta.DAL.Model;

namespace WpfConta {
    public class RowStatus :IValueConverter{
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            /*
            var item = value as UiBase;
            if (item == null) return 0;
            return (item.IsDirty ? 1 : 0) +
                (item.IsLocked ? 2 : 0);
            /* */
            /*
            var item = value as IUiProjectItem;
            if (item == null) return false;

            if (item.RowType == ProjectRowType.Category)
                return true;

            var parent = MainWdw.ProjectItems.FirstOrDefault(x => x.Parent == item.Parent);
            return !(parent != null && parent.HasDetails);
            /* */
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
