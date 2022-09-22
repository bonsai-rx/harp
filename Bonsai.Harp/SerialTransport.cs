using System;
using System.IO.Ports;

namespace Bonsai.Harp
{
    class SerialTransport : StreamTransport, IDisposable
    {
        const int DefaultBaudRate = 1000000;
        const int DefaultReadBufferSize = 1048576; // 2^20 = 1 MB
        readonly SerialPort serialPort;
        bool disposed;

        public SerialTransport(string portName, IObserver<HarpMessage> observer)
            : base(observer)
        {
            serialPort = new SerialPort(portName, DefaultBaudRate, Parity.None, 8, StopBits.One);
            serialPort.ReadBufferSize = DefaultReadBufferSize;
            serialPort.Handshake = Handshake.RequestToSend;
            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.ErrorReceived += serialPort_ErrorReceived;
        }

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //TODO: Create exception with the error state and send to observer
        }

        public void Open()
        {
            serialPort.Open();
        }

        public void Write(HarpMessage input)
        {
            serialPort.Write(input.MessageBytes, 0, input.MessageBytes.Length);
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try { ReceiveData(serialPort.BaseStream, serialPort.ReadBufferSize, serialPort.BytesToRead); }
            catch (InvalidOperationException) { }
        }

        public void Close()
        {
            if (!disposed)
            {
                serialPort.Dispose();
                disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}
