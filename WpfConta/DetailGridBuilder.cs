#define UseBorder
#define CacheMeasurings_
using Conta.Dal;
using Conta.Dal.Model;
using Conta.Model;
using Conta.UiController;
using Conta.UiController.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using WpfConta.Converters;

namespace WpfConta {
    public class DetailGridBuilder : IDisposable {
        public static DetailGridBuilder Build(Grid host, Border border, Type dataType, DataGrid dataSourceGrid) {
            var result = new DetailGridBuilder(host, border, dataSourceGrid);
            result.DoBuild(dataType);
            return result;
        }

        private const double MarginSize = 2d;

        private readonly Grid theGrid;
#if(UseBorder)
        private readonly Border border;
#endif
        private readonly DataGrid dataSourceGrid;
        private readonly List<Record> DataSource;
#if(CacheMeasurings)
        private readonly List<double> gridWidth = new List<double>();
#else
#endif
        private readonly Window window;

        private DetailGridBuilder(Grid host, Border border, DataGrid dataSourceGrid) {
            this.theGrid = host;
#if(UseBorder)
            this.border = border;
            if (this.border != null) this.border.SizeChanged += host_SizeChanged;
#endif
            this.dataSourceGrid = dataSourceGrid;
            this.dataSourceGrid.SizeChanged += host_SizeChanged;

            //host.SizeChanged += host_SizeChanged;
            object p = host.Parent;
            while (p != null) {
                if (p is Window) {
                    window = p as Window;
                    window.SizeChanged += host_SizeChanged;
                    break;
                }

                if (p is Page) {
                    var page = p as Page;
                    page.SizeChanged += host_SizeChanged;
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
            foreach (Control child in host.Children) {
                if (child is TextBox)
                    BindingOperations.ClearBinding(child, TextBox.TextProperty);
                else if (child is DatePicker)
                    BindingOperations.ClearBinding(child, DatePicker.SelectedDateProperty);
            }
            host.Children.Clear();
        }

        private double GetWidth() {
            object p = this.theGrid.Parent;
            while (p != null) {
                if (p is Window)
                    return (p as Window).ActualWidth;

                if (p is Page)
                    return (p as Page).ActualWidth;

                var parentProperty = p.GetType().GetProperty("Parent");
                if (parentProperty == null)
                    break;

                p = parentProperty.GetValue(p, null);
            }

            return 0d;
        }

        private void DoBuild(Type dataType) {
            //theGrid.Children.Clear();
            Debug.WriteLine("DetailGridBuilder.Build(" + dataType.Name + ")");
            //var measure = new TextBox();
            //theGrid.Children.Add(measure);

            // build
            foreach (var prop in dataType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)) {
                var forwardKeyInfo = new ForwardKeyInfo { PropName = prop.Name };
                var colName = prop.Name;
                var attrs = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
                if (attrs.Length > 0)
                    if (!(attrs[0] as BrowsableAttribute).Browsable) {
                        // not browsable
                        if (typeof(IUiBase).IsAssignableFrom(dataType)) {
                            var originalType = dataType.GetField("original", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
                            if (originalType != null) {
                                var originalProperty = originalType.FieldType.GetProperty(prop.Name);
                                if (originalProperty != null) {
                                    attrs = originalProperty.GetCustomAttributes(typeof(ForeignKeyAttribute), true);
                                    if (attrs.Length > 0) {
                                        forwardKeyInfo.TargetType = typeof(IDalData).Assembly.GetType("Conta.DAL.Model." + (attrs[0] as ForeignKeyAttribute).Name);
                                        if (forwardKeyInfo.TargetType != null) {
                                            // infer ui target type
                                            foreach (var type in typeof(AppServices).Assembly.GetTypes()) {
                                                var originalField = type.GetField("original", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
                                                if (originalField != null &&
                                                    forwardKeyInfo.TargetType == originalField.FieldType) {
                                                    forwardKeyInfo.UiType = type;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (forwardKeyInfo.TargetType == null)
                            continue;   // Browse(false) & not a foreign key
                    }

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
                    var dt = new DatePicker {
                        Name = "datePicker_" + prop.Name,
                        IsTabStop = true,
                        //Background = Brushes.Azure,
                    };

                    dt.SetBinding(DatePicker.SelectedDateProperty, ApplyBinding(prop.Name, isReadOnly));
                    editor = dt;
                } else if (forwardKeyInfo.TargetType != null) {
                    // "known" class : ckeck for foreign key
                    var ctrl = new Button();
                    ctrl.Content = prop.Name;
                    ctrl.Click += ctrlForeignKey_Click;
                    ctrl.Tag = forwardKeyInfo;
                    //ctrl.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    //display.EditorWidth = ctrl.ActualWidth;

                    //ctrl.SetBinding(Button.TagProperty, ApplyBinding(string.Empty, isReadOnly));
                    editor = ctrl;
                } else if ((attrs = prop.GetCustomAttributes(typeof(LookupBoundPropertyAttribute), true)).Length > 0) {
                    var lookupAttribute = attrs[0] as LookupBoundPropertyAttribute;
                    var combo = new ComboBox {
                        Name = "comboBox_" + prop.Name,
                        DisplayMemberPath = lookupAttribute.DisplayMember,
                        SelectedValuePath = lookupAttribute.LookupMember,
                    };

                    combo.SetBinding(ComboBox.SelectedValueProperty, ApplyBinding(prop.Name, isReadOnly));
                    if (UiProjectStatus.Service == null) UiProjectStatus.InitService();
                    combo.ItemsSource = UiProjectStatus.Service.GetList();

                    editor = combo;
                } else {
                    // catch-all
                    var textLen = 10;
                    //if (prop.PropertyType == typeof(int)) textLen = 10;
                    if (prop.PropertyType == typeof(string)) {
                        attrs = prop.GetCustomAttributes(typeof(StringLengthAttribute), true);
                        if (attrs.Length > 0)
                            textLen = (attrs[0] as StringLengthAttribute).MaximumLength;
                    }

                    var isForeignValue = prop.PropertyType.IsClass && prop.PropertyType != typeof(string);
                    isReadOnly |= isForeignValue;

                    var ctrl = new TextBox {
                        //Background = Brushes.Azure,
                        Name = "textBox_" + prop.Name,
                        //Text = new string('n', textLen),
                        IsReadOnly = isReadOnly,
                    };
                    //measure.Text = new string('n', textLen);
                    //measure.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));     --> 0
                    //ctrl.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));     --> 0
                    //display.EditorWidth = ctrl.ActualWidth;
                    //Debug.WriteLine("width: " + ctrl.ActualWidth);
                    //Debug.WriteLine("width: " + measure.ActualWidth);

                    var binding = ApplyBinding(prop.Name, isReadOnly);
                    if (isForeignValue)
                        binding.Converter = new ToStringConverter();

                    ctrl.SetBinding(TextBox.TextProperty, binding);
                    editor = ctrl;
                }

                if (string.IsNullOrEmpty(editor.Name))
                    editor.Name = "detail_" + dataType.Name + "_" + prop.Name;
                var display = new Record(/*dataType.Name + */colName, editor);
                DataSource.Add(display);
                display.AddToGrid(theGrid);
            }

            //theGrid.Children.Remove(measure);

            InitGrid(theGrid);
            Debug.WriteLine("init");
            Rearrange(theGrid.ActualWidth);
            Debug.WriteLine("end init");
        }

        void ctrlForeignKey_Click(object sender, RoutedEventArgs e) {
            if (dataSourceGrid.SelectedItem == null) return;

            var button = sender as Button;
            Debug.Assert(button != null && button.Tag is ForwardKeyInfo);
            var forwardKeyInfo = button.Tag as ForwardKeyInfo;

            var dlg = new BoSelector();
            var childController = new EditableListSelectorController(dlg, forwardKeyInfo.UiType);

            //childController.Setter = x => (button.DataContext as DefaultCustomController).SelectedItem.GetType().GetProperty(propName).SetValue(button.DataContext, x);
            childController.Setter = x => SetForwardProperty(forwardKeyInfo, x);

            dlg.DataContext = childController;
            dlg.ShowDialog();
        }

        private void SetForwardProperty(ForwardKeyInfo forwardKeyInfo, object newValue) {
            var destProperty = dataSourceGrid.SelectedItem.GetType().GetProperty(forwardKeyInfo.PropName);
            if (destProperty == null) return;
            foreach (var keyProperty in forwardKeyInfo.TargetType.GetProperties()) {
                var attrs = keyProperty.GetCustomAttributes(typeof(KeyAttribute));
                foreach (var attr in attrs) {
                    var uiProperty = forwardKeyInfo.UiType.GetProperty(keyProperty.Name);
                    var keyValue = uiProperty.GetValue(newValue);
                    destProperty.SetValue(dataSourceGrid.SelectedItem, keyValue);
#warning will only work on single-column key
                    return;
                }
            }

            Debug.Assert(false, "No primary key found");
        }

        private bool IsKnownType(Type type) {
            //var result = BusinessObject.Declared.Exists(x => x.UiDataType == type);
            var result = typeof(IDalData).IsAssignableFrom(type);
            return result;
        }

        private int lastColCount = -1;
        public void Rearrange(double width = 0d) {
            System.Threading.ThreadPool.QueueUserWorkItem(w =>
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => TreadRearrange(w))), width);
        }

        private void TreadRearrange(object status){
            //Debug.WriteLine("Rearrange");
            if (DataSource == null)
                return;     // not initialised
            var width = status == null ? 0d : (double)status;

            // TODO : make it abortable
            var gridWidth = theGrid.ActualWidth - theGrid.Margin.Left - theGrid.Margin.Right;

#if(UseBorder)
            var targetWidth = Math.Min(gridWidth, GetWidth());
#else
            var targetWidth = gridWidth;
#endif
            if (width >= 0d)
                targetWidth = Math.Min(targetWidth, width);

            //var targetWidth = gridWidth;
            if (targetWidth <= 0d) return;

#if(UseBorder)
            //Debug.WriteLine("Rearrange: " + gridWidth + " / " + targetWidth);
#else
            Debug.WriteLine("Rearrange: " + gridWidth);
#endif
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
            // DoEvents
            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            var totalWidth = 0d;
            var maxDesc = new double[colCount + 1];
            var maxCtrl = new double[colCount + 1];
            for (var i = 0; i <= colCount; i++) {
                maxDesc[i] = 0d;
                maxCtrl[i] = 0d;
            }
            for (var colIndex = 0; colIndex < DataSource.Count; colIndex++) {
                var colNo = colIndex % (colCount + 1);
                //var rowNo = colIndex / (colCount + 1);

                maxDesc[colNo] = Math.Max(maxDesc[colNo], DataSource[colIndex].DescWidth);
                maxCtrl[colNo] = Math.Max(maxCtrl[colNo], DataSource[colIndex].EditorWidth);
                //Debug.WriteLine(string.Format("{0},{3}: {1} / {2}", colIndex, DataSource[colIndex].DescWidth, DataSource[colIndex].EditorWidth, rowNo));
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
            theGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            theGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            for (var colGroupNo = 1; colGroupNo < colCount; colGroupNo++) {
                theGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                theGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                theGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            }
            /*
            foreach (var col in theGrid.ColumnDefinitions)
                Debug.Write(col.Width.GridUnitType + "(" + col.ActualWidth + ") ");
            Debug.WriteLine(".");
            /* */

            for (var colIndex = 0; colIndex < DataSource.Count; colIndex++) {
                var colNo = colIndex % colCount;
                var rowNo = colIndex / colCount;
                //theGrid.Children.Remove(DataSource[colIndex].DescriptionLabel);
                var initialColumn = 0;
                var initialRow = 0;
                if (DataSource[colIndex].DescriptionLabel != null) {
                    initialColumn = (int)DataSource[colIndex].DescriptionLabel.GetValue(Grid.ColumnProperty);
                    initialRow = (int)DataSource[colIndex].DescriptionLabel.GetValue(Grid.RowProperty);
                    DataSource[colIndex].DescriptionLabel.SetValue(Grid.ColumnProperty, 3 * colNo);
                    DataSource[colIndex].DescriptionLabel.SetValue(Grid.RowProperty, rowNo);
                    //theGrid.Children.Add(DataSource[colIndex].DescriptionLabel);
                }
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
            /*
            {
                // DoEvents
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { 
                    var width = 0d;
                    foreach (var col in theGrid.ColumnDefinitions)
                        width += col.ActualWidth;
                    Debug.WriteLine("grid width: " + width + " / " + colCount);
                }));
            }/* */
        }

        private void InitGrid(Grid grid) {
            foreach (var rec in DataSource) {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                if (rec.DescriptionLabel != null) rec.DescriptionLabel.Visibility = Visibility.Visible;
                rec.DataCtrl.Visibility = Visibility.Visible;
            }
        }

        private void host_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e) {
            //Debug.WriteLine("SizeChanged " + sender.GetType().Name + " : " + e.NewSize.Width);
            Rearrange(e.NewSize.Width);
        }

        private Binding ApplyBinding(string propName, bool isReadOnly) {
            var binder = new Binding();
            binder.Mode = isReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            // bind to the property of the selected item in the data grid
            binder.ElementName = dataSourceGrid.Name;
            binder.Path = new PropertyPath("SelectedItem" + (string.IsNullOrWhiteSpace(propName) ? string.Empty : ("." + propName)));
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
            public double DescWidth { get { return GetWidth(this.Description, this.DescriptionLabel) + 2d * MarginSize; } }
            public double EditorWidth { get { return GetEditWidth(); } }
#endif

            private double GetEditWidth() {
                return this.DataCtrl.DesiredSize.Width;
                //return this.DataCtrl.ActualWidth;
            }

            public void AddToGrid(Grid grid) {
                if (!(this.DataCtrl is Button)) {
                    this.DescriptionLabel = new Label {
                        Content = this.Description,
                        Margin = new Thickness(MarginSize, 0d, MarginSize, 0d),
                        //Background = Brushes.Fuchsia,
                    };
                    //this.DescriptionWidth = this.DescriptionLabel.ActualWidth;    // 0.0
#if(CacheMeasurings)
                this.DescWidth = GetWidth(this.Description, this.DescriptionLabel) + 10d;
#else
#endif
                    this.DescriptionLabel.Name = "lbl" + this.DataCtrl.Name;
                    this.DescriptionLabel.Visibility = Visibility.Collapsed;

                    grid.Children.Add(this.DescriptionLabel);
                }

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
                return ctrl == null ?
                    0d :
                    new FormattedText(text,
                        CultureInfo.CurrentUICulture,
                        FlowDirection.LeftToRight,
                        new Typeface(ctrl.FontFamily, ctrl.FontStyle, ctrl.FontWeight, ctrl.FontStretch),
                        ctrl.FontSize,
                        Brushes.Black).Width;
            }
        }

        class ForwardKeyInfo {
            public string PropName { get; set; }
            public Type UiType { get; set; }
            public Type TargetType { get; set; }
        }
    }
}
