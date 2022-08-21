using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Telemetry_App.DBControllers;
using Telemetry_App.SerialControllers;
using Telemetry_App.Models;
using Newtonsoft.Json;

namespace Telemetry_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SerialPortsCountMonitor serialPortsCountMonitor;

        private SerialController serialController;

        private DBController dbController;

        private List<DBTableModel> dbTablesModels;
        private List<DBTableModel> testDBTablesModels;

        private List<SerialDataModel> serialDataModels;

        private DataSet serialData;

        private string serverName = ConfigurationManager.AppSettings.Get("DBServer");
        private string dataBaseName = ConfigurationManager.AppSettings.Get("DBDataBase");
        private string testDataBaseName = ConfigurationManager.AppSettings.Get("DBTestDataBase");

        private char serialDataSeperator = ConfigurationManager.AppSettings.Get("SerialDataSeperator").ToCharArray()[0];

        public App()
        {
            serialController = new SerialController(OnSerialDataRecived);
            serialPortsCountMonitor = new SerialPortsCountMonitor(OnNewSerialPortFound);

            string testDBTablesModelsJsonString = File.ReadAllText(@".\ConfigurableDataModels\TestDBTablesModels.json");
            string dbTablesModelsJsonString = File.ReadAllText(@".\ConfigurableDataModels\DBTablesModels.json");
            string serialDataModelJsonString = File.ReadAllText(@".\ConfigurableDataModels\SerialDataModel.json");

            dbTablesModels = JsonConvert.DeserializeObject<List<DBTableModel>>(dbTablesModelsJsonString);
            testDBTablesModels = JsonConvert.DeserializeObject<List<DBTableModel>>(testDBTablesModelsJsonString);
            serialDataModels = JsonConvert.DeserializeObject<List<SerialDataModel>>(serialDataModelJsonString);

            DBCreator.CrateDB(serverName, dataBaseName);
            DBCreator.CrateDB(serverName, testDataBaseName);

            serialData = new DataSet();
            foreach (var dataTableModel in serialDataModels)
            {
                serialData.Tables.Add(new DataTable(dataTableModel.FrameID));
                foreach (var dataID in dataTableModel.DataIDs)
                {
                    serialData.Tables[dataTableModel.FrameID].Columns.Add(new DataColumn(dataID, typeof(string)));
                }
                serialData.Tables[dataTableModel.FrameID].Rows.Add(serialData.Tables[dataTableModel.FrameID].NewRow());
            }
            DBTableCrator.CreateTables(serverName, dataBaseName, dbTablesModels);
            DBTableCrator.CreateTables(serverName, dataBaseName, testDBTablesModels);
        }

        ~App()
        {
            DBCreator.DestroyDB(serverName, dataBaseName);
            DBCreator.DestroyDB(serverName, testDataBaseName);

            serialPortsCountMonitor.Dispose();
            serialController.Dispose();
            dbController.Dispose();
        }

        private void OnSerialDataRecived(string data)
        {
            List<string> recivedData = data.Split(serialDataSeperator).ToList();

            if (recivedData.Count < 1) return;
            if (recivedData[0].Length != 1) return;
            string frameID = recivedData[0];
            if (serialData.Tables[frameID].Columns.Count != recivedData[0].Length - 1) return;

            DataRow dataRow = serialData.Tables[frameID].Rows[0];
            for (int i = 1; i < recivedData.Count; i++)
            {
                dataRow[i - 1] = recivedData[i];
            }
        }

        private void OnNewSerialPortFound(string port)
        {
            if (serialController.IsOpen) serialController.Close();
            serialController.PortName = port;
            serialController.Open();
            MessageBox.Show($"Connected App to port {port}");
        }
    }
}
