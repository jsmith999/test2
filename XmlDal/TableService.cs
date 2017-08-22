using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XmlDal {
    public abstract class TableService<TTable, TKey> : Conta.DAL.BaseTableService<TTable, TKey> where TTable : class {
        private static List<PropertyInfo> properties = new List<PropertyInfo>();

        protected LambdaExpression ByPrimaryKey<T>(T key, Type resultType, string keyName = "") {
            if (string.IsNullOrWhiteSpace(keyName)) keyName = this.KeyName;

            var fieldType = typeof(T);
            var pe = Expression.Parameter(resultType);
            var filterBody = Expression.Equal(Expression.Property(pe, keyName), Expression.Constant(key, fieldType));
            var filter = Expression.Lambda(filterBody, new[] { pe });
            return filter;
        }

        public TTable FromKey(TKey key) {
            return GetList(ByPrimaryKey(key, typeof(TTable))).FirstOrDefault();
        }

        protected override IEnumerable<TTable> DoGetList(LambdaExpression where = null, string toSearch = null) {
            var table = Load();
            var result = new List<TTable>();

            if (where == null) {
                foreach (DataRow row in table.Rows) {
                    var item = DataToModel(row);
                    if (IsInSearch(item, toSearch))
                        result.Add(item);
                }
            } else {
                var compiled = where.Compile();
                foreach (DataRow row in table.Rows) {
                    var item = DataToModel(row);
                    if (IsInSearch(item, toSearch) &&
                        (bool)compiled.DynamicInvoke(new[] { item }))
                        result.Add(item);
                }
            }

            return result;
        }

        private bool IsInSearch(TTable model, string toSearch) {
            if (string.IsNullOrEmpty(toSearch))
                return true;

            lock (((ICollection)properties).SyncRoot) {
                if (properties.Count == 0) {
                    foreach (var prop in typeof(TTable).GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                        properties.Add(prop);
                }
            }

            foreach (var prop in properties) {
                var value = prop.GetValue(model);
                if (value != null && value.ToString().Contains(toSearch))
                    return true;
            }

            return false;
        }

        protected override TTable DoCreate() {
            var table = Load();
            var newRow = table.NewRow();
            // deal with non-nulls
            foreach (DataColumn col in table.Columns) {
                if (col.DefaultValue != null)
                    newRow[col] = col.DefaultValue;

                if (!col.AllowDBNull && newRow[col] is DBNull) {
                    if (col.AutoIncrement) {
                        if (table.Rows.Count == 0)
                            newRow[col] = 1;
                        else {
                            var lastRow = table.Rows[table.Rows.Count - 1];
                            newRow[col] = (int)lastRow[col] + 1;
                        }

                        continue;
                    }

                    if (col.DataType == typeof(string))
                        newRow[col] = string.Empty;
                    else if (col.DataType == typeof(DateTime))
                        newRow[col] = DateTime.MinValue;
                    else
                        Debugger.Break();       // missed one
                }
            }
            //
            table.Rows.Add(newRow);
            Save(table);
            return DataToModel(newRow);
        }

        protected override bool DoUpdate(TTable item) {
            var table = Load();
            var row = FindRow(table, item);
            if (row == null)
                return false;

            row.BeginEdit();
            ModelToData(item, row);
            try {
                row.EndEdit();
                Save(table);
                return true;
            } catch (ArgumentException argEx) {
                Trace.TraceError("ArgumentException : " + argEx.Message);
                row.CancelEdit();
                DataToModel(item, row);
            }

            return false;
        }

        protected override TTable DoDelete(TTable item) {
            var table = Load();
            var row = FindRow(table, item);
            if (row == null)
                return null;

            //table.Rows.Remove(row);
            row.Delete();

            Save(table);
            return item;
        }

        protected abstract TTable DataToModel(DataRow row);
        protected abstract void DataToModel(TTable item, DataRow row);
        protected abstract void ModelToData(TTable item, DataRow row);

        protected abstract DataRow FindRow(DataTable table, TTable item);

        protected virtual DataTable Load() {
            var result = new DataTable(TableName);
            var fileName = TableName + ".xml";
            var path = System.IO.Path.GetFullPath(fileName);
            result.ReadXml(fileName);
            return result;
        }

        private void Save(DataTable table) {
            table.WriteXml(TableName + ".xml", XmlWriteMode.WriteSchema);
        }
    }
}
