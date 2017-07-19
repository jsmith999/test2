#define CacheMeasurings_
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfConta {
    public class DetailGridBuilder : IDisposable {
        public static DetailGridBuilder Build(Grid host, Border border, Type dataType, DataGrid dataSourceGrid) {
            var result = new DetailGridBuilder(host, border, dataSourceGrid);
            result.DoBuild(dataType);
            return result;
        }

        private const double MarginSize = 5d;

        private readonly Grid theGrid;
        private readonly Border border;
        private readonly DataGrid dataSourceGrid;
        private readonly List<Record> DataSource;
#if(CacheMeasurings)
        private readonly List<double> gridWidth = new List<double>();
#else
#endif
        private readonly Window window;

        private DetailGridBuilder(Grid host, Border border, DataGrid dataSourceGrid) {
            this.theGrid = host;
            this.border = border;
            this.dataSourceGrid = dataSourceGrid;

            //host.SizeChanged += host_SizeChanged;
            object p = host.Parent;
            while (p != null) {
                if (p is Window) {
                    window = p as Window;
                    window.SizeChanged += host_SizeChanged;
                    break;
                }

                var parentProperty = p.GetType().GetProperty("Parent");
                if (parentProperty == null)
                    break;

                p = parentProperty.GetValue(p, null);
            }

            DataSource = new List<Record>();

            //host.Children.Clear();    // not good enough
            ClearHostChildren(host);
            host.ColumnDefinitions.Clear();
        }

        private void ClearHostChildren(Grid host) {
            foreach(Control child in host.Children){
                if (child is TextBox)
                    BindingOperations.ClearBinding(child, TextBox.TextProperty);
                else if (child is DatePicker)
                    BindingOperations.ClearBinding(child, DatePicker.SelectedDateProperty);
            }
            host.Children.Clear();
        }

        private void DoBuild(Type dataType) {
            //theGrid.Children.Clear();
            Debug.WriteLine("DetailGridBuilder.Build(" + dataType.Name + ")");

            // build
            foreach (var prop in dataType.GetProperties( BindingFlags.GetProperty|BindingFlags.Instance|BindingFlags.Public)) {
                var colName = prop.Name;
                var attrs = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
                if (attrs.Length > 0)
                    if (!(attrs[0] as BrowsableAttribute).Browsable)
                        continue;       // not browsable

                attrs = prop.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (attrs.Length > 0)
                    colName = (attrs[0] as DisplayNameAttribute).DisplayName;

                var isReadOnly = false;
                attrs = prop.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
                if (attrs.Length > 0)
                    isReadOnly = (attrs[0] as ReadOnlyAttribute).IsReadOnly;
                else
                    isReadOnly = !prop.CanWrite;

                Control editor = null;
                if (prop.PropertyType == typeof(DateTime)) {
                    var dt = new DatePicker();
                    dt.SetBinding(DatePicker.SelectedDateProperty, ApplyBinding(prop.Name, isReadOnly));
                    editor = dt;
                } else {
                    var textLen = 10;
                    //if (prop.PropertyType == typeof(int)) textLen = 10;
                    if (prop.PropertyType == typeof(string)) {
                        attrs = prop.GetCustomAttributes(typeof(StringLengthAttribute), true);
                        if (attrs.Length > 0)
                            textLen = (attrs[0] as StringLengthAttribute).MaximumLength;
                    }

                    var ctrl = new TextBox();
                    ctrl.Text = new string('n', textLen);
                    //ctrl.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    //display.EditorWidth = ctrl.ActualWidth;

                    ctrl.SetBinding(TextBox.TextProperty, ApplyBinding(prop.Name, isReadOnly));
                    editor = ctrl;
                }

                editor.Name = "detail_" + dataType.Name + "_" + prop.Name;
                var display = new Record(/*dataType.Name + */colName, editor);
                DataSource.Add(display);
                display.AddToGrid(theGrid);
            }

            InitGrid(theGrid);
            Rearrange();
        }

        private int lastColCount = -1;
        public void Rearrange() {
            //Debug.WriteLine("Rearrange");
            if (DataSource == null)
                return;     // not initialised

            // TODO : make it abortable
            var gridWidth = theGrid.ActualWidth - theGrid.Margin.Left - theGrid.Margin.Right;

            var targetWidth = Math.Min(gridWidth, border.ActualWidth);
            //var targetWidth = gridWidth;
            if (targetWidth <= 0d) return;

            //Debug.WriteLine("SizeChanged: " + gridWidth + " / " + border.ActualWidth);
            for (var colCount = 0; colCount < DataSource.Count; colCount++) {
                //Debug.WriteLine(string.Format("CalculateWidth({0})", colCount));
                var totalWidth = CalculateWidth(colCount);
                //Debug.WriteLine(string.Format("col count:{0}, width:{1}", colCount, totalWidth));
                if (totalWidth > targetWidth) {
                    GoSettle(gridWidth, colCount);
                    return;
                }
            }

            GoSettle(gridWidth, DataSource.Count);
        }

        private void GoSettle(double gridWidth, int colCount) {
            if (colCount == lastColCount)
                return;

            lastColCount = colCount;
            //Debug.WriteLine("SizeChanged: " + gridWidth + " / " + border.ActualWidth + " -> " + lastColCount + " " + (theGrid.ColumnDefinitions.Count >= 3 ? theGrid.ColumnDefinitions[2].ActualWidth.ToString() : "X"));
            Settle(colCount);
            //Debug.WriteLine("end SizeChanged: " + theGrid.ActualWidth); // no recalc yet
        }

        /// <summary>
        /// Calculate the width of the grid.
        /// </summary>
        /// <param name="colCount">Number of column groups : zero based (0 for 1 column group)</param>
        /// <returns></returns>
        private double CalculateWidth(int colCount) {
#if(CacheMeasurings)
            if (gridWidth.Count > colCount && gridWidth[colCount] > 0d)
                return gridWidth[colCount];
#else
#endif

            var totalWidth = 0d;
            var maxDesc = new double[colCount + 1];
            var maxCtrl = new double[colCount + 1];
            for (var i = 0; i <= colCount; i++) {
                maxDesc[i] = 0d;
                maxCtrl[i] = 0d;
            }
            for (var colIndex = 0; colIndex < DataSource.Count; colIndex++) {
                var colNo = colIndex % (colCount + 1);
                var rowNo = colIndex / (colCount + 1);

                maxDesc[colNo] = Math.Max(maxDesc[colNo], DataSource[colIndex].DescWidth);
                maxCtrl[colNo] = Math.Max(maxCtrl[colNo], DataSource[colIndex].EditorWidth);
                //Debug.WriteLine(string.Format("{0}: {1} / {2}", colIndex, DataSource[colIndex].DescriptionWidth, DataSource[colIndex].DataWidth));
            }
            //Debug.WriteLine(string.Format("{0}: {1} x {2}", colNo, maxDesc, maxCtrl));
            for (var i = 0; i <= colCount; i++)
                totalWidth = totalWidth + maxDesc[i] + maxCtrl[i];

            totalWidth += 2d * MarginSize * (colCount + 1);
            //Debug.WriteLine(string.Format("CalculateWidth : {0} -> {1}", colCount, totalWidth));
#if(CacheMeasurings)
            //Debug.Assert(gridWidth.Count == colCount - 1);
            while (gridWidth.Count < colCount)
                gridWidth.Add(-1d);
#else
#endif

#if(CacheMeasurings)
            gridWidth.Add(totalWidth);
            Debug.Assert(gridWidth[colCount] == totalWidth);
#else
#endif
            return totalWidth;
        }

        /// <summary>
        /// Rearrange the controls based on the number of column groups.
        /// </summary>
        /// <param name="colCount">Number of column groups (1 based).</param>
        private void Settle(int colCount) {
            Debug.Assert(colCount > 0);
            var maxColumns = 3 * colCount - (colCount <= 1 ? 0 : 1);
            if (theGrid.ColumnDefinitions.Count == maxColumns)
                return;

            //Debug.WriteLine("Settle: " + colCount + " [" + theGrid.ColumnDefinitions.Count + "x" + maxColumns + "]");
            theGrid.ColumnDefinitions.Clear();
            for (var colGroupNo = 0; colGroupNo < colCount; colGroupNo++) {
                theGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                theGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                theGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            if (colCount > 1)
                theGrid.ColumnDefinitions.RemoveAt(theGrid.ColumnDefinitions.Count - 1);
            /*
            foreach (var col in theGrid.ColumnDefinitions)
                Debug.Write(col.Width.GridUnitType + "(" + col.ActualWidth + ") ");
            Debug.WriteLine(".");
            /* */

            for (var colIndex = 0; colIndex < DataSource.Count; colIndex++) {
                var colNo = colIndex % colCount;
                var rowNo = colIndex / colCount;
                //theGrid.Children.Remove(DataSource[colIndex].DescriptionLabel);
                var initialColumn = (int)DataSource[colIndex].DescriptionLabel.GetValue(Grid.ColumnProperty);
                var initialRow = (int)DataSource[colIndex].DescriptionLabel.GetValue(Grid.RowProperty);
                DataSource[colIndex].DescriptionLabel.SetValue(Grid.ColumnProperty, 3 * colNo);
                DataSource[colIndex].DescriptionLabel.SetValue(Grid.RowProperty, rowNo);
                //theGrid.Children.Add(DataSource[colIndex].DescriptionLabel);

                //theGrid.Children.Remove(DataSource[colIndex].DataCtrl);
                DataSource[colIndex].DataCtrl.SetValue(Grid.ColumnProperty, 3 * colNo + 1);
                DataSource[colIndex].DataCtrl.SetValue(Grid.RowProperty, rowNo);
                //theGrid.Children.Add(DataSource[colIndex].DataCtrl);
                /*
                                Debug.WriteLine(DataSource[colIndex].Description + " : " +
                                    initialColumn + "x" + initialRow + " -> " +
                                    (3 * colNo) + "x" + rowNo + " / " +
                                    theGrid.Children.IndexOf(DataSource[colIndex].DescriptionLabel) + "/" +
                                    DataSource[colIndex].DescriptionLabel.ActualWidth + "/" +
                                    DataSource[colIndex].DataCtrl.ActualWidth + "/" +
                                    DataSource[colIndex].DescriptionLabel.Visibility + " / " + DataSource[colIndex].DataCtrl.Visibility);/* */
            }
            {
                var rowNo = DataSource.Count % (colCount + 1);
                //Debug.WriteLine(string.Format("using {0} rows out of {1}", rowNo, theGrid.RowDefinitions.Count));
                while (theGrid.RowDefinitions.Count <= rowNo)
                    theGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            }
            /*
            Debug.Write("rows:");
            foreach (var row in theGrid.RowDefinitions)
                Debug.Write(" " + row.ActualHeight);
            Debug.WriteLine(".");   /* */
        }

        private void InitGrid(Grid grid) {
            foreach (var rec in DataSource) {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                rec.DescriptionLabel.Visibility = Visibility.Visible;
                rec.DataCtrl.Visibility = Visibility.Visible;
            }
        }

        void host_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e) {
            Rearrange();
        }

        private Binding ApplyBinding(string propName, bool isReadOnly) {
            var binder = new Binding();
            binder.Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            // bind to the property of the selected item in the data grid
            binder.ElementName = dataSourceGrid.Name;
            binder.Path = new PropertyPath("SelectedItem." + propName);
            binder.ValidatesOnDataErrors = true;
            return binder;
        }

        #region IDisposable Members
        public void Dispose() {
            theGrid.SizeChanged -= host_SizeChanged;
            if (window != null)
                window.SizeChanged -= host_SizeChanged;
        }
        #endregion

        class Record {
            public Record(string description, Control dataCtrl) {
                Description = description;
                DataCtrl = dataCtrl;
            }

            public string Description { get; set; }
            public Control DataCtrl { get; set; }
            public Label DescriptionLabel { get; set; }
#if(CacheMeasurings)
            public double DescWidth { get; set; }
            public double EditorWidth { get; set; }
#else
            public double DescWidth { get { return GetWidth(this.Description, this.DescriptionLabel) + this.DescriptionLabel.Margin.Right + this.DescriptionLabel.Margin.Left; } }
            public double EditorWidth { get { return GetEditWidth(); } }
#endif

            private double GetEditWidth() {
                return this.DataCtrl.DesiredSize.Width;
                //return this.DataCtrl.ActualWidth;
            }

            public void AddToGrid(Grid grid) {
                this.DescriptionLabel = new Label { Content = this.Description, Margin = new Thickness(MarginSize), };
                //this.DescriptionWidth = this.DescriptionLabel.ActualWidth;    // 0.0
#if(CacheMeasurings)
                this.DescWidth = GetWidth(this.Description, this.DescriptionLabel) + 10d;
#else
#endif
                this.DescriptionLabel.Name = "lbl" + this.DataCtrl.Name;
                this.DescriptionLabel.Visibility = Visibility.Collapsed;

                grid.Children.Add(this.DescriptionLabel);
                grid.Children.Add(this.DataCtrl);

                if (this.DataCtrl is TextBox) {
                    //(this.DataCtrl as TextBox).Text = new string('n', 20);
#if(CacheMeasurings)
                    this.EditorWidth = GetWidth((this.DataCtrl as TextBox).Text, this.DataCtrl);
#else
#endif
                } else if (this.DataCtrl is DatePicker) {
                    this.DataCtrl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    // not working
#if(CacheMeasurings)
                    this.EditorWidth = this.DataCtrl.DesiredSize.Width;
#else
#endif
                }
                //Debug.WriteLine("init [" + Description + "] width : " + this.DescWidth + " / " + this.EditorWidth);
                this.DataCtrl.Visibility = Visibility.Collapsed;
            }

            private double GetWidth(string text, Control ctrl) {
                return new FormattedText(text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(ctrl.FontFamily, ctrl.FontStyle, ctrl.FontWeight, ctrl.FontStretch),
                    ctrl.FontSize,
                    Brushes.Black).Width;
            }
        }
    }
}
