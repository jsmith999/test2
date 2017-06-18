using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Conta.DAL;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Conta.Controller;

namespace Conta {
    public partial class DetailCtrl : UserControl {
        private List<PropertyDisplay> Properties;
        private bool ignoreResize;

        public DetailCtrl() {
            InitializeComponent();
        }

        public BindingSource BindingSource { get; set; }

        public void Attach(Type type) {
            this.Controls.Clear();
            Properties = new List<PropertyDisplay>();

            foreach (var prop in type.GetProperties()) {
                var colName = prop.Name;
                var attrs = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
                if (attrs.Length > 0)
                    if(!(attrs[0] as BrowsableAttribute).Browsable)
                        continue;       // not browsable

                attrs = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (attrs.Length > 0)
                    colName = (attrs[0] as DisplayNameAttribute).DisplayName;

                var display = new PropertyDisplay();
                Properties.Add(display);
                display.Description = new Label();
                display.Description.Text = colName;
                this.Controls.Add(display.Description);
                display.DescWidth = TextRenderer.MeasureText(colName, display.Description.Font).Width;
                display.Description.Width = display.DescWidth;

                // TODO : add more controls here
                if (prop.PropertyType == typeof(DateTime)) {
                    var dt = new DateTimePicker();
                    display.Editor = dt;
                    display.EditorWidth = dt.Width;
                    display.Editor.DataBindings.Add("Value", this.BindingSource, prop.Name);
                } else {
                    var textLen = 10;
                    //if (prop.PropertyType == typeof(int)) textLen = 10;
                    if (prop.PropertyType == typeof(string)) {
                        attrs = prop.GetCustomAttributes(typeof(StringLengthAttribute), true);
                        if (attrs.Length > 0)
                            textLen = (attrs[0] as StringLengthAttribute).MaximumLength;
                    }

                    var dt = new TextBox();
                    display.Editor = dt;
                    display.EditorWidth = TextRenderer.MeasureText(new string('n', textLen), display.Editor.Font).Width;
                    display.Editor.Width = display.EditorWidth;
                    display.Editor.DataBindings.Add("Text", this.BindingSource, prop.Name);
                }

                this.Controls.Add(display.Editor);
            }

            RearrangeColumns();
        }

        private void RearrangeColumns() {
            if (Properties == null)
                return;     // not initialised

            ignoreResize = true;
            // TODO : make it abortable
            try {
                for (int cols = Properties.Count; cols >= 1; cols--) {
                    if (RearrangeColumns(cols))
                        return;
                }
            } finally {
                ignoreResize = false;
            }
        }

        private bool RearrangeColumns(int colsCount) {
            var colsWidth = new int[colsCount];
            var descWidth = new int[colsCount];

            {
                var maxDesc = 0;
                var maxEdit = 0;
                var colNo = 0;
                var lastColNo = 0;
                for (var pdIndex = 0; pdIndex < Properties.Count; pdIndex++) {
                    colNo = pdIndex * colsCount / Properties.Count;
                    if (colNo != lastColNo) {
                        colsWidth[lastColNo] = maxDesc + maxEdit;
                        descWidth[lastColNo] = maxDesc;

                        maxDesc = 0;
                        maxEdit = 0;
                        lastColNo = colNo;
                    }
                    var pd = Properties[pdIndex];
                    maxDesc = Math.Max(maxDesc, pd.DescWidth);
                    maxEdit = Math.Max(maxEdit, pd.EditorWidth);
                }
                colsWidth[colNo] = maxDesc + maxEdit;
                descWidth[colNo] = maxDesc;
            }

            var totalWidth = 0;
            for (var col = 0; col < colsWidth.Length; col++)
                totalWidth += colsWidth[col];

            const int LeftMargin = 5;
            const int RightMargin = 5;
            var colsDist = colsCount <= 1 ? 0 : (this.Width - totalWidth - LeftMargin - RightMargin) / (colsCount - 1);
            if (colsDist < 0) return false;

            //Debug.WriteLine("colsDist " + colsDist + "\t" + this.Width + "\t" + totalWidth);
            var descStart = LeftMargin;
            var maxHeight = 0;
            
            {
                const int TopMargin = 5;
                const int BottomMargin = 5;

                var top = TopMargin;
                var lastCol = 0;
                for (var pdIndex = 0; pdIndex < Properties.Count; pdIndex++) {
                    var colNo = pdIndex * colsCount / Properties.Count;
                    if (lastCol != colNo) {
                        descStart += colsWidth[lastCol] + colsDist;
                        top = TopMargin;

                        lastCol = colNo;
                    }

                    var pd = Properties[pdIndex];
                    pd.Description.Left = descStart;
                    pd.Description.Top = top;

                    pd.Editor.Left = descStart + descWidth[colNo];
                    pd.Editor.Top = top;

                    top += Math.Max(pd.Description.Height, pd.Editor.Height);
                    //Debug.WriteLine(pd.Description.Text + "\t" + pd.Description.Left + "\t" + pd.Description.Width + "\t" +
                    //    pd.Description.Top + "\t" + pd.Editor.Left + "\t" + pd.Editor.Width);
                    maxHeight = Math.Max(maxHeight, top);
                }
                maxHeight += BottomMargin;
            }

            this.Height = maxHeight;
            return true;
        }

        class PropertyDisplay {
            public Label Description { get; set; }
            public Control Editor { get; set; }
            public int DescWidth { get; set; }
            public int EditorWidth { get; set; }
        }

        private void DetailCtrl_Resize(object sender, EventArgs e) {
            if (!ignoreResize)
                RearrangeColumns();
        }

        internal void SetRowStatus(Conta.UiController.Controller.RowStatus status) {
            SetReadOnly(status == Conta.UiController.Controller.RowStatus.Locked);
        }

        internal void SetReadOnly(bool isReadOnly) {
            foreach (var item in Properties)
                item.Editor.Enabled = !isReadOnly;
        }
    }
}
