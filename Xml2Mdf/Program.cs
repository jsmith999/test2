using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Xml2Mdf {
    class Program {
        const string SourcePath = @"..\..\..\";
        const string ConnStr = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\sources\Conta\BaseLine.mdf;Integrated Security=True;Connect Timeout=30";
        const string TableRootPath = "NewDataSet/xs:schema/xs:element/xs:complexType/xs:choice/xs:element";

        static void Main(string[] args) {
            using (var conn = new SqlConnection(ConnStr)) {
                conn.Open();
                var trans = conn.BeginTransaction();

                try {
                    foreach (var fn in Directory.GetFiles(Path.GetFullPath(SourcePath), "*.xml")) {
                        var tableName = CreateTable(fn, conn, trans);
                        CopyData(fn, conn, trans, tableName);
                    }

                    trans.Commit();
                } catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                    trans.Rollback();
                } finally {
                    trans.Dispose();
                }
            }
        }

        private static string CreateTable(string fn, SqlConnection conn, SqlTransaction trans) {
            Debug.WriteLine("loading " + fn);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(fn);
            var ns = new XmlNamespaceManager(xmlDoc.NameTable);
            ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            ns.AddNamespace("msdata", "urn:schemas-microsoft-com:xml-msdata");

            var query = new StringBuilder();
            var result = xmlDoc.SelectSingleNode(TableRootPath + "/@name", ns).InnerText;
            query.AppendFormat("CREATE TABLE [dbo].[{0}](\r\n", result);
            foreach (XmlNode col in xmlDoc.SelectNodes(TableRootPath + "/xs:complexType/xs:sequence/xs:element", ns)) {
                // get type
                var attr = col.Attributes.GetNamedItem("type");
                var xsType = "";
                if (attr != null) xsType = attr.InnerText;

                if (string.IsNullOrEmpty(xsType)) {
                    attr = col.SelectSingleNode("xs:simpleType/xs:restriction/@base", ns);
                    if (attr != null)
                        xsType = attr.InnerText;
                }

                var columnType = xsType;
                if (xsType == "xs:int") {
                    columnType = "[int]";

                    attr = col.SelectSingleNode("@msdata:AutoIncrement", ns);
                    if (attr != null && attr.InnerText == "true")
                        columnType += " IDENTITY(1,1)";
                } else if (xsType == "xs:string") {
                    columnType = "[nvarchar]";
                    attr = col.SelectSingleNode("xs:simpleType/xs:restriction/xs:maxLength/@value", ns);
                    if (attr != null)
                        columnType += "(" + attr.InnerText + ")";
                } else if (xsType == "xs:dateTime") {
                    columnType = "[datetime]";
                } else if (xsType == "xs:double") {
                    columnType = "[numeric](18,0)";
                } else if (xsType == "xs:boolean") {
                    columnType = "[bit]";
                } else {
                    throw new Exception("Unknown type : " + xsType);
                }

                var isNull = "NOT NULL";
                attr = col.SelectSingleNode("@minOccurs");
                if (attr != null && attr.InnerText == "0")
                    isNull = "NULL";

                query.AppendFormat("[{0}]{1} {2},\r\n",
                    col.Attributes.GetNamedItem("name").InnerText,
                    columnType,
                    isNull);
            }
            query.Append(")");
            /* */
            var cmd = conn.CreateCommand();
            cmd.Transaction = trans;
            cmd.CommandText = query.ToString();
            cmd.ExecuteNonQuery();
            /* */
            return result;
        }

        private static Dictionary<Type, SqlDbType> Mapper = new Dictionary<Type, SqlDbType> {
            { typeof(int), SqlDbType.Int },
            { typeof(string), SqlDbType.NVarChar },
            { typeof(DateTime), SqlDbType.DateTime },
            { typeof(double), SqlDbType.Float },
            { typeof(bool), SqlDbType.Bit },
            //{ typeof(int), SqlDbType.Int },
        };

        private static void CopyData(string fn, SqlConnection conn, SqlTransaction trans, string tableName) {
            var dt = new DataTable(tableName);
            dt.ReadXml(fn);
            //dt.AcceptChanges();

            var insertCommand = conn.CreateCommand();
            var query = new StringBuilder("insert into [" + tableName + "](");
            var values = new StringBuilder(") values (");
            var filter = new StringBuilder(") where ");
            foreach (DataColumn col in dt.Columns) {
                if (col.ReadOnly)
                    filter.AppendFormat("{0} = @{0} AND", col.ColumnName);
                else {
                    query.AppendFormat("[{0}], ", col.ColumnName);
                    values.AppendFormat("@{0}, ", col.ColumnName);
                }

                insertCommand.Parameters.Add("@" + col.ColumnName, Mapper[col.DataType], col.MaxLength, col.ColumnName);
            }

            query.Remove(query.Length - 2, 2);
            values.Remove(values.Length - 2, 2);
            filter.Remove(filter.Length - 4, 4);
            insertCommand.CommandText = query.ToString() + values.ToString() + ")";/*+ filter.ToString();*/
            insertCommand.Transaction = trans;

            var adapter = new SqlDataAdapter("select * from " + tableName, conn);
            adapter.InsertCommand = insertCommand;

            adapter.Update(dt);
        }
    }
}
