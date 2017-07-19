using Conta.Dal;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Conta.UiController.Controller {
    public interface IController : INotifyPropertyChanged {
        Type CurrentType { get; }
        //Action<ObservableCollection<IUiBase>> SetViewDataSource { get; set; }
        Action<ICollection> SetViewDataSource { get; set; }
        bool IsGlobalSearch { get; set; }
        bool HasParent { get; set; }

        void AddNew();
        void DeleteSelection();
        bool CanClose();
        void SetDataType(Type type, UiBase parent);
        void SelectionChanged(IUiBase item);
        void SetChildFilter(IUiBase item);
        void Search(string searchValue);
        void ExecuteReport(string header);
    }
}
