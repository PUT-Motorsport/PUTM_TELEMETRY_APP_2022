using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telemetry_App.DBControllers;
using Telemetry_App.SerialControllers;
using Telemetry_App.Models;

namespace Telemetry_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPortsCountMonitor serialPortsCountMonitor;

        private SerialController serialController;

        private DBController DBController;

        public MainWindow()
        {
            InitializeComponent();

            serialController = new SerialController(OnSerialDataRecived);
            serialPortsCountMonitor = new SerialPortsCountMonitor(OnNewSerialPortFound);
        }

        public void OnSerialDataRecived(string data)
        {

        }

        public void OnNewSerialPortFound(string port)
        {
            if (serialController.IsOpen) serialController.Close();
            serialController.PortName = port;
            serialController.Open();
        }
    }
}
