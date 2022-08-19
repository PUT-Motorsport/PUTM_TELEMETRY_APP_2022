using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Windows;

namespace Telemetry_App.SerialControllers
{
    class SerialPortsCountMonitor : IDisposable
    {
        private Thread thread;

        private List<string> oldPorts;

        private Action<string> onNewPortFoundCallback;

        public SerialPortsCountMonitor(Action<string> onNewPortFoundCallback)
        {
            oldPorts = new List<string>(SerialPort.GetPortNames());
            this.onNewPortFoundCallback = onNewPortFoundCallback;

            thread = new Thread(Monitor);

            thread.Start();
        }

        public void Dispose()
        {
            thread.Abort();
            thread.Join();
        }

        private void Monitor()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(100);

                    List<string> ports = new List<string>(SerialPort.GetPortNames());

                    foreach (string port in ports)
                    {
                        string value = oldPorts.Find((p) => p == port);
                        if (!string.IsNullOrWhiteSpace(value)) continue;

                        onNewPortFoundCallback(port);
                    }

                    oldPorts = ports;
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
