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

        private CancellationTokenSource tokenSource;

        public SerialPortsCountMonitor(Action<string> onNewPortFoundCallback)
        {
            oldPorts = new List<string>(SerialPort.GetPortNames());
            this.onNewPortFoundCallback = onNewPortFoundCallback;
            tokenSource = new CancellationTokenSource();

            thread = new Thread(() => Monitor(tokenSource.Token));

            thread.Start();
        }

        public void Dispose()
        {
            tokenSource.Cancel();
            thread.Join();
        }

        private void Monitor(CancellationToken token)
        {
            try
            {
                while (true)
                { 
                    Thread.Sleep(100);

                    token.ThrowIfCancellationRequested();

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
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
