using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Data.SqlClient;
using Telemetry_App.Models;

namespace Telemetry_App.DBControllers
{
    class DBTableCrator
    {
        public static void CreateTable(string server, string dataBase, string tableToCreate, List<ColumnModel> sqlCollumnModels, bool addTimeCollumn = true)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={dataBase};Trusted_Connection=True;"))
            {
                try
                {
                    connection.Open();
                    StringBuilder commandBuilder = new StringBuilder();
                    //commandBuilder.Append(@"IF OBJECT_ID(N'").Append(tableToCreate).AppendLine(@"', N'U') IS NOT NULL");
                    //commandBuilder.Append("BEGIN DROP TABLE ").Append(tableToCreate).AppendLine(" END");
                    commandBuilder.Append("CREATE TABLE ").Append(tableToCreate).AppendLine();
                    commandBuilder.AppendLine("(");
                    if (addTimeCollumn)
                    {
                        commandBuilder.AppendLine("\ttime DATETIME DEFAULT GETDATE(),");
                    }
                    foreach (var columnModel in sqlCollumnModels)
                    {
                        commandBuilder.Append("\t").Append(columnModel.ColumnName).Append(" ");
                        commandBuilder.Append(columnModel.DataType);
                        if (columnModel.Default != null) commandBuilder.Append(" DEFAULT ").Append(columnModel.Default);
                        commandBuilder.AppendLine(",");
                    }
                    commandBuilder.AppendLine(")");
                    commandBuilder.Append("GRANT SELECT ON ").Append(tableToCreate).Append(" TO public");
                    SqlCommand command = new SqlCommand(commandBuilder.ToString(), connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public static void CreateTables(string serverName, string dataBaseName, List<DBTableModel> tableModels)
        {
            foreach (var tableModel in tableModels)
            {
                CreateTable(serverName, dataBaseName, tableModel.TableName, tableModel.Columns.ToList());
            }
        }
    }
}
