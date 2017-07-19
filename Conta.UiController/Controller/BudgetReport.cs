using HTMLReportEngine;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Conta.UiController.Controller {
    class BudgetReport {
        private const string ConnStrFormat = @"Data Source=(LocalDB)\v11.0;AttachDbFilename={0}\BaseLine.mdf;Integrated Security=True;Connect Timeout=30";

        public string Create() {
            using (SqlConnection connection =
                       new SqlConnection(string.Format(ConnStrFormat, Path.GetFullPath(@"..\..\..")))) {

                var adapter = new SqlDataAdapter();
                adapter.TableMappings.Add("Table", "Report");
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM project p left outer join client c on c.id = p.clientkey;", connection);
                command.CommandType = CommandType.Text;

                // Set the SqlDataAdapter's SelectCommand.
                adapter.SelectCommand = command;
                var dataSet = new DataSet("Report");
                adapter.Fill(dataSet);
                var report = new Report {
#if(false)
                    IncludeChart = false,
#else
                    IncludeChart = true,
                    ChartTitle = "chart",
                    ChartValueField = "budget",
                    //ChartValueHeader = "chart budget",
                    //ChartLabelHeader ="chart header",
                    ChartShowAtBottom = true,
                    ChartChangeOnField = "LastName", // Unable to Generate Chart. 'name' argument cannot be null. Parameter name: name
#endif
                    IncludeTotal = true,
                    //TotalFields = new System.Collections.ArrayList{ new Field("budget", "Budget"), },     // no

                    ReportFields = new System.Collections.ArrayList { 
                        new Field("Name", "Name"),
                        new Field("budget", "Budget"){ isTotalField = true, },
                        new Field("FirstName", "First Name"),
                        new Field("LastName", "Last Name"),
                    },
                    ReportSource = dataSet,
                    ReportTitle = "My demo",
                };

                var rpt = report.GenerateReport();
                rpt = rpt.Substring(0, rpt.IndexOf("</html>", StringComparison.InvariantCultureIgnoreCase) + 7);
                return rpt;
                //Debug.WriteLine(rpt);
                //var saved = report.SaveReport(@"C:\sources\HtmlReportDemo\bin\Debug\demo.html");
                //Debug.WriteLine(saved);
            }
        }
    }
}
