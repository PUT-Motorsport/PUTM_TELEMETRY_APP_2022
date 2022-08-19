using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Windows;

namespace Telemetry_App.DBControllers
{
    class DBCreator
    {
        public static void CrateDB(string serverName, string dataBaseToCrate)
        {
            using(SqlConnection connection = new SqlConnection($"Server={serverName};Trusted_Connection=True;"))
            {
                try
                {
                    StringBuilder commandBuilder = new StringBuilder();
                    commandBuilder.Append("IF DB_ID(").Append(dataBaseToCrate).AppendLine(" IS NOT NULL");
                    commandBuilder.Append("\tDROP DATABASE ").AppendLine(dataBaseToCrate);
                    commandBuilder.Append("CREATE DATABASE").AppendLine(dataBaseToCrate);
                    SqlCommand command = new SqlCommand(commandBuilder.ToString(), connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public static void DestroyDB(string serverName, string dataBaseToDestroy)
        {
            using (SqlConnection connection = new SqlConnection($"Server={serverName};Trusted_Connection=True;"))
            {
                try
                {
                    StringBuilder commandBuilder = new StringBuilder();
                    commandBuilder.Append($"DROP DATABASE {dataBaseToDestroy}\n");
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
