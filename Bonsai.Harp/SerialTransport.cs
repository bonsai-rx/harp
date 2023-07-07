using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    class SerialTransport : StreamTransport, IDisposable
    {
        const int DefaultBaudRate = 1000000;
        const int DefaultReadBufferSize = 1048576; // 2^20 = 1 MB
        readonly CancellationTokenSource taskCancellation;
        readonly SerialPort serialPort;

        public SerialTransport(string portName, IObserver<HarpMessage> observer)
            : base(observer)
        {
            IgnoreErrors = true;
            taskCancellation = new CancellationTokenSource();
            serialPort = new SerialPort(portName, DefaultBaudRate, Parity.None, 8, StopBits.One);
            serialPort.ReadBufferSize = DefaultReadBufferSize;
            serialPort.Handshake = Handshake.RequestToSend;
            RunAsync(taskCancellation.Token);
        }

        Task RunAsync(CancellationToken cancellationToken)
        {
            serialPort.Open();
            return Task.Factory.StartNew(() =>
            {
                using var cancellation = cancellationToken.Register(serialPort.Dispose);
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var bytesToRead = serialPort.BytesToRead;
                        if (bytesToRead == 0)
                        {
                            PushData(serialPort.BaseStream, serialPort.ReadBufferSize, count: 1);
                            bytesToRead = serialPort.BytesToRead;
                        }

                        ReceiveData(serialPort.BaseStream, serialPort.ReadBufferSize, bytesToRead);
                    }
                    catch (Exception ex)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            OnError(ex);
                        }
                        break;
                    }
                }
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        public void Write(HarpMessage input)
        {
            serialPort.Write(input.MessageBytes, 0, input.MessageBytes.Length);
        }

        public void Close()
        {
            if (!taskCancellation.IsCancellationRequested)
            {
                taskCancellation.Cancel();
                taskCancellation.Dispose();
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}
