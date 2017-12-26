using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    class SerialTransport : StreamTransport, IDisposable
    {
        const int DefaultReadBufferSize = 1048576; // 2^20 = 1 MB
        readonly SerialPort serialPort;
        bool disposed;

        public SerialTransport(string portName, IObserver<HarpDataFrame> observer)
            : base(observer)
        {
            //serialPort = new SerialPort(portName, 2000000, Parity.None, 8, StopBits.One);
            serialPort = new SerialPort(portName, 1000000, Parity.None, 8, StopBits.One);
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

        public void Write(HarpDataFrame input)
        {
            serialPort.Write(input.Message, 0, input.Message.Length);
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ReceiveData(serialPort.BaseStream, serialPort.ReadBufferSize, serialPort.BytesToRead);
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
