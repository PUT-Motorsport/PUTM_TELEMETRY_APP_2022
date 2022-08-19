using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Data.SqlClient;
using Telemetry_App.Models;

namespace Telemetry_App.DBControllers
{
    class DBTableCrator
    {
        public static void CreateTable(string server, string dataBase, string tableToCreate, List<SqlCollumnModel> sqlCollumnModels)
        {
            using (SqlConnection connection = new SqlConnection($"Server={server};Database={dataBase};Trusted_Connection=True;"))
            {
                try
                {
                    StringBuilder commandBuilder = new StringBuilder();
                    commandBuilder.Append("CREATE TABLE ").Append(tableToCreate).AppendLine();
                    commandBuilder.AppendLine("(");
                    foreach(var collumnModel in sqlCollumnModels)
                    {
                        commandBuilder.Append("\t");
                        commandBuilder.Append(collumnModel.collumnName).Append(" ");
                        commandBuilder.Append(collumnModel.dataType).Append(" ");
                        if (collumnModel.defaultData != null) commandBuilder.Append("DEFAULT").Append(collumnModel.defaultData);
                        commandBuilder.AppendLine();
                    }
                    commandBuilder.AppendLine(")");
                    commandBuilder.Append("GRANT SELECT ON ").Append(tableToCreate).Append(" TO public");
                    SqlCommand command = new SqlCommand(commandBuilder.ToString(), connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
