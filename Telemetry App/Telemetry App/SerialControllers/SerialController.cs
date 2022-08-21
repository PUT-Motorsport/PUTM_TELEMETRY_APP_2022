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
        private char frameSeperator;

        private StringBuilder recivedDataBuilder;

        private Action<string> onFrameRecivedCallback;

        public SerialController(Action<string> onFrameRecivedCallback)
        {
            BaudRate = int.Parse(ConfigurationManager.AppSettings.Get("SerialBaudRate"));
            Parity = (Parity)Enum.Parse(typeof(Parity), ConfigurationManager.AppSettings.Get("SerialParity"));
            DataBits = int.Parse(ConfigurationManager.AppSettings.Get("SerialDataBits"));
            StopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigurationManager.AppSettings.Get("SerialStopBits"));
            Encoding = Encoding.GetEncoding(ConfigurationManager.AppSettings.Get("SerialEncoding"));
            Handshake = (Handshake)Enum.Parse(typeof(Handshake), ConfigurationManager.AppSettings.Get("SerialHandshake"));
            RtsEnable = false;

            frameSeperator = ConfigurationManager.AppSettings.Get("SerialFrameSeperator").ToCharArray()[0];

            recivedDataBuilder = new StringBuilder();

            this.onFrameRecivedCallback = onFrameRecivedCallback;

            ErrorReceived += (o, e) => Close();
            DataReceived += OnDataRecived;
        }

        ~SerialController()
        {
            Dispose();
        }

        private void OnDataRecived(object sender, SerialDataReceivedEventArgs e)
        {
            while(BytesToRead > 0)
            {
                char character = (char)ReadChar();
                if (character == frameSeperator)
                {
                    onFrameRecivedCallback(recivedDataBuilder.ToString());
                    recivedDataBuilder.Clear();
                }
                else
                {
                    recivedDataBuilder.Append(character);
                }
            }
        }
    }
}
