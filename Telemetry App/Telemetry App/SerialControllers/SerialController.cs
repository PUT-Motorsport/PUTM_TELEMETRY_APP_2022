using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Telemetry_App.SerialControllers
{
    class SerialController : SerialPort
    {
        public SerialController(Action<string> onDataRecivedCallback)
        {
            BaudRate = int.Parse(ConfigurationManager.AppSettings.Get("SerialBaudRate"));
            Parity = (Parity)Enum.Parse(typeof(Parity), ConfigurationManager.AppSettings.Get("SerialParity"));
            DataBits = int.Parse(ConfigurationManager.AppSettings.Get("SerialDataBits"));
            StopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigurationManager.AppSettings.Get("SerialStopBits"));
            Encoding = Encoding.GetEncoding(ConfigurationManager.AppSettings.Get("SerialEncoding"));
            Handshake = (Handshake)Enum.Parse(typeof(Handshake), ConfigurationManager.AppSettings.Get("SerialHandshake"));
            RtsEnable = false;

            ErrorReceived += (o, e) => Close();
            DataReceived += (o, e) => onDataRecivedCallback(ReadExisting());
        }
    }
}
