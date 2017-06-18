using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Conta.Controller;
using Conta.Model;
using System.Diagnostics;
using Conta.Dal;
using Conta.UiController.Controller;

namespace Conta {
    public partial class frmMain : Form, IMainView {
        MainController controller;
        SyncBindings binder;

        public frmMain() {
            InitializeComponent();

            // required
            theDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            binder = new SyncBindings(theBindingSource, theDataGridView);
            theDetail.BindingSource = theBindingSource;
            controller = new MainController(this);
        }

        private void FileExitMenu_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = !controller.CanClose();
        }

        #region IMainView Members
        public object GridDataSource {
            set {
                theDataGridView.DataSource = value;
                theBindingSource.DataSource = value;
            }
        }

        public bool GridReadOnly { 
            set { 
                theDataGridView.ReadOnly = value;
                theDetail.SetReadOnly(value);
            } 
        }

        public int DetailDataSourceIndex {
            set {
                foreach (DataGridViewRow selected in theDataGridView.SelectedRows)
                    selected.Selected = selected.Index == value;

                theBindingSource.Position = value;
            }
        }

        public void SetRowStatus(int index, RowStatus status) {
            //Debug.WriteLine(string.Format("SetRowStatus({0}, {1})", index, status));

            theDataGridView.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            switch (status) {
                case RowStatus.Editing:
                    if (theDataGridView.Rows[index].Selected)
                        theDataGridView.DefaultCellStyle.SelectionBackColor = Color.DarkRed;
                    else
                        theDataGridView.Rows[index].DefaultCellStyle.ForeColor = Color.Aqua;
                    break;

                case RowStatus.Locked:
                    if (theDataGridView.Rows[index].Selected)
                        theDataGridView.DefaultCellStyle.SelectionBackColor = Color.LightGray;
                    else
                        theDataGridView.Rows[index].DefaultCellStyle.ForeColor = Color.LightGray;
                    break;

                case RowStatus.Normal:
                    theDataGridView.Rows[index].DefaultCellStyle.ForeColor = theDataGridView.ForeColor;
                    break;
            }

            if (theDataGridView.SelectedRows.Count != 1)
                return;

            var selectedItem = theDataGridView.SelectedRows[0];
            if (selectedItem.Index == index)
                theDetail.SetRowStatus(status);

        }

        public void SetDetail(Type type) {
            theDetail.Attach(type);
        }

        public object GridDetailSource {
            set { throw new NotImplementedException(); }
        }

        public void SelectGrid(int gridType) { /*TO DO*/ }
        public event EventHandler<TypeEventArgs> JumpToData;        
        #endregion

        private void RaiseJumpToData(Type arg) {
            if (JumpToData != null)
                JumpToData(this, new TypeEventArgs(arg));
        }

        private void ClientsMenu_Click(object sender, EventArgs e) {
            RaiseJumpToData(typeof(UiClient));
        }

        private void EmployeesMenu_Click(object sender, EventArgs e) {
            RaiseJumpToData(typeof(UiEmployee));
        }

        private void theDataGridView_SelectionChanged(object sender, EventArgs e) {
            var grid = sender as DataGridView;
            var newSelected = grid.SelectedRows.Count == 0 ?
                null :
                grid.SelectedRows[0].DataBoundItem as IUiBase;

            controller.SelectionChanged(newSelected);
        }

        private void DataAddNewMenu_Click(object sender, EventArgs e) {
            controller.AddNew();
        }
    }
}
