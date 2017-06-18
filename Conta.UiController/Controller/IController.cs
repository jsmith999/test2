using Conta.Dal;
using System;
using System.Collections;
using System.ComponentModel;

namespace Conta.UiController.Controller {
    public interface IController : INotifyPropertyChanged {
        Type CurrentType { get; }
        Action<ICollection> SetViewDataSource { get; set; }
        bool IsGlobalSearch { get; set; }
        bool HasParent { get; set; }

        void AddNew();
        bool CanClose();
        void SetDataType(Type type, UiBase parent);
        void SelectionChanged(IUiBase item);
        void SetChildFilter(IUiBase item);
        void Search(string searchValue);
    }
}
