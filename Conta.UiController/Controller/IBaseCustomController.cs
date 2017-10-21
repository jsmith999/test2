using Conta.Dal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Conta.UiController.Controller {
    public interface IBaseCustomController {
        Type CurrentType { get; }
        //Action<ObservableCollection<IUiBase>> SetViewDataSource { get; set; }
        Action<ICollection> SetViewDataSource { get; set; }
        bool IsGlobalSearch { get; set; }
        bool HasParent { get; set; }

        UiBase AddNew();
        void DeleteSelection();
        void SelectionChanged(IUiBase item);
        void SetChildFilter(IUiBase item);
        void Search(string searchValue);
        void ExecuteReport(string header);

        bool CanClose();
        void SetDataType(Type type, UiBase parent);
        void CloseCurrentService();
    }

    public interface IDetailCustomView : IBaseCustomView {
        IEnumerable GridDetailSource { get; set; }
        void DetailSelection(IUiBase item);
    }

    public interface IBaseCustomView {
        object DataContext { get; set; }

        //DataGrid MainGrid { get; }
        string SearchValue { get; }
        object SelectedItem { get; set; }
        object GridDataSource { set; }
        bool GridReadOnly { set; }

        event EventHandler AddBOItem;
        event EventHandler DelBOItem;
        event EventHandler StartSearch;
        event EventHandler<UiBase> MainGridSelectionChanged;
        //event EventHandler ForwardListSelected;

        //void SetSelection(IUiBase item);
        //void SetRowStatus(int oldSelectionIndex, RowStatus lockStatus);
        MessageActions ShowMessage(string title, string message, MessageActions action);
        //void SetReports(IEnumerable<string> reports);
    }
}
