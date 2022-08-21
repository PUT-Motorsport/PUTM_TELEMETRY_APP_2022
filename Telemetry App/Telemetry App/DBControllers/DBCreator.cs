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
                    connection.Open();
                    StringBuilder commandBuilder = new StringBuilder();
                    commandBuilder.Append(@"IF DB_ID(N'").Append(dataBaseToCrate).AppendLine(@"') IS NOT NULL");
                    commandBuilder.Append("BEGIN DROP DATABASE ").Append(dataBaseToCrate).AppendLine(" END");
                    commandBuilder.Append("CREATE DATABASE ").AppendLine(dataBaseToCrate);
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

        public static void DestroyDB(string serverName, string dataBaseToDestroy)
        {
            using (SqlConnection connection = new SqlConnection($"Server={serverName};Trusted_Connection=True;"))
            {
                try
                {
                    connection.Open();
                    StringBuilder commandBuilder = new StringBuilder();
                    commandBuilder.Append($"ALTER DATABASE ").Append(dataBaseToDestroy).AppendLine(" SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                    commandBuilder.Append($"DROP DATABASE {dataBaseToDestroy}\n");
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
    }
}
