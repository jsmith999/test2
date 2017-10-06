using Conta.Dal;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Conta.UiController.Controller {
    public interface IDataController : INotifyPropertyChanged {
        /*
        //Action<ObservableCollection<IUiBase>> SetViewDataSource { get; set; }
        bool IsGlobalSearch { get; set; }
        Type CurrentType { get; }
        void SetDataType(Type type, UiBase parent);
        void AddNew();
        void DeleteSelection();/* */
        void SelectionChanged(IUiBase item);
        void Search(string searchValue);
        bool HasParent { get; set; }

        Action<ICollection> SetViewDataSource { get; set; }
        void SetChildFilter(IUiBase item);
        void ExecuteReport(string header);

        bool CanClose();
    }
}
