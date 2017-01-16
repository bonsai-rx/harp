using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    public class SerialTransport : IDisposable
    {
        const byte IdMask = 0x03;
        readonly IObserver<HarpDataFrame> observer;
        readonly SerialPort serialPort;
        readonly CircularPort circularPort;
        byte[] currentMessage;
        int currentOffset;
        int pendingId;
        bool disposed;

        public SerialTransport(string portName, IObserver<HarpDataFrame> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            this.observer = observer;
            serialPort = new SerialPort(portName, 2000000, Parity.None, 8, StopBits.One);
            serialPort.RtsEnable = true;
            serialPort.DataReceived += serialPort_DataReceived;
            circularPort = new CircularPort(serialPort);
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
            var bytesToRead = serialPort.BytesToRead;
            circularPort.PushBytes(bytesToRead);
            while (serialPort.IsOpen && bytesToRead > 0)
            {
                // There is a current packet
                if (currentMessage != null)
                {
                    var remaining = Math.Min(currentMessage.Length - currentOffset, bytesToRead);
                    var bytesRead = circularPort.Read(currentMessage, currentOffset, remaining);

                    currentOffset += bytesRead;
                    bytesToRead -= bytesRead;

                    // If our packet is complete
                    if (currentOffset >= currentMessage.Length)
                    {
                        byte sum = 0;
                        var checksum = currentMessage[currentMessage.Length - 1];
                        for (int i = 0; i < currentMessage.Length - 1; i++)
                        {
                            sum += currentMessage[i];
                        }

                        // If checksum is valid, emit packet
                        if (sum == checksum)
                        {
                            var dataFrame = new HarpDataFrame(currentMessage);
                            observer.OnNext(dataFrame);
                        }
                        else
                        {
                            var offset = currentMessage.Length - 2;
                            circularPort.Seek(-offset);
                            bytesToRead += offset;
                        }
                        currentMessage = null;
                        currentOffset = 0;
                        pendingId = 0;
                    }
                }
                // Read packet length and allocate
                else if (pendingId > 0)
                {
                    var length = circularPort.ReadByte();
                    if (length > 0)
                    {
                        currentMessage = new byte[length + 2];
                        currentMessage[0] = (byte)pendingId;
                        currentMessage[1] = (byte)length;
                        currentOffset = 2;
                    }
                    else pendingId = 0;
                    bytesToRead--;
                }
                // Check for a new packet
                else
                {
                    pendingId = circularPort.ReadByte();
                    if ((pendingId & ~IdMask) != 0)
                    {
                        pendingId = 0;
                    }

                    bytesToRead--;
                }
            }
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
