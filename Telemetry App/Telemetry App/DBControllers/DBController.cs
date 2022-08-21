using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Telemetry_App.DBControllers;
using Telemetry_App.SerialControllers;
using Telemetry_App.Models;

namespace Telemetry_App.DBControllers
{
    class DBController : IDisposable
    {
        private string serverName;
        private string dbName;

        private Thread dbUpdater;

        private DataSet serialData;

        private DataSet tables;

        private List<DBTableModel> dbTablesModels;

        private List<SerialDataModel> serialDataModels;

        private CancellationTokenSource tokenSource;

        public DBController(string serverName, string dbName, DataSet serialData, List<DBTableModel> dbTablesModels, List<SerialDataModel> serialDataModels)
        {
            this.serialData = serialData;
            this.dbTablesModels = dbTablesModels;
            this.serialDataModels = serialDataModels;
            this.dbName = dbName;
            this.serverName = serverName;

            dbUpdater = new Thread(() => DBUpdater(tokenSource.Token));
            dbUpdater.Start();
        }
    
        ~DBController()
        {
            Dispose();
        }

        private void DBUpdater(CancellationToken token)
        {
            var dbTableHandlersList = new List<Task>();

            foreach (var tableModel in dbTablesModels)
            {
                var bindedValues = new List<SerialDataBinder>();

                foreach(var columnModel in tableModel.Columns)
                {
                    // FIXME: tableModel.TableName it should be the frameID
                    //bindedValues.Add((string)serialData.Tables[tableModel.TableName].Rows[0][columnModel.ColumnName]);
                    //string value = serialData.
                    int index = -1;
                    string frameID = "";
                    foreach (var serialDataModel in serialDataModels)
                    {
                        if(serialData.Tables[serialDataModel.FrameID].Columns.Contains(columnModel.ColumnName))
                        {
                            index = serialData.Tables[serialDataModel.FrameID].Columns.IndexOf(columnModel.ColumnName);
                            frameID = serialDataModel.FrameID;
                            break;
                        }
                    }
                    if (index == -1) ;
                    SerialDataBinder serialDataBinder = new SerialDataBinder();
                    serialDataBinder.binding = (string)serialData.Tables[frameID].Rows[0][index];
                    serialDataBinder.columnName = columnModel.ColumnName;
                    bindedValues.Add(serialDataBinder);
                }

                dbTableHandlersList.Add(new Task(() => UpdateDB(tableModel.UpdateRateMs, tableModel.TableName, bindedValues)));
            }

            foreach (var tableHandler in dbTableHandlersList)
            {
                tableHandler.Start();
            }

            while (true)
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    Thread.Sleep(1);

                    foreach(var tableHandler in dbTableHandlersList)
                    {
                        if (tableHandler.IsCompleted) tableHandler.Start();
                    }
                }
                catch (OperationCanceledException) 
                {
                    return;
                } 
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }

        private async Task UpdateDB(int interval, string tableName, List<SerialDataBinder> bindedValues)
        {
            await Task.Delay(interval);
            using (SqlConnection connection = new SqlConnection($"Server={serverName};Database={dbName};Trusted_Connection=True;"))
            {
                try
                {
                    connection.Open();
                    StringBuilder commandBuilder = new StringBuilder();
                    commandBuilder.Append("INSERT INTO ").Append(tableName).Append(" ( ");
                    for (int i = 0; i < bindedValues.Count - 1; i++)
                    {
                        commandBuilder.Append(bindedValues[i].columnName).Append(" ,");
                    }
                    commandBuilder.Append(bindedValues[bindedValues.Count - 1]).AppendLine(" )");
                    commandBuilder.Append("VALUES ( ");
                    for (int i = 0; i < bindedValues.Count - 1; i++)
                    {
                        commandBuilder.Append(bindedValues[i].binding).Append(" ,");
                    }
                    commandBuilder.Append(bindedValues[bindedValues.Count - 1]).AppendLine(" );");

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

        class SerialDataBinder
        {
            public string binding;
            public string columnName;
        }

        public void Dispose()
        {
            tokenSource.Cancel();
            dbUpdater.Join();
        }
    }
}
