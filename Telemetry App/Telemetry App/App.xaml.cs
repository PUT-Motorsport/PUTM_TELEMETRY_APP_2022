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
        public App()
        {
        }
    }
}
