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
        const int DefaultReadBufferSize = 1048576; // 2^20 = 1 MB
        const byte IdMask = 0x03;
        const byte ErrorMask = 0x08;
        readonly IObserver<HarpDataFrame> observer;
        readonly SerialPort serialPort;
        BufferedStream bufferedStream;
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
            try
            {
                var bytesToRead = serialPort.BytesToRead;
                bufferedStream = bufferedStream ?? new BufferedStream(serialPort.BaseStream, serialPort.ReadBufferSize);
                bufferedStream.PushBytes(bytesToRead);
                while (serialPort.IsOpen && bytesToRead > 0)
                {
                    // There is a current packet
                    if (currentMessage != null)
                    {
                        var remaining = Math.Min(currentMessage.Length - currentOffset, bytesToRead);
                        var bytesRead = bufferedStream.Read(currentMessage, currentOffset, remaining);

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
                                bufferedStream.Seek(-offset);
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
                        var length = bufferedStream.ReadByte();
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
                        pendingId = bufferedStream.ReadByte();
                        if ((pendingId & ~(IdMask | ErrorMask)) != 0)
                        {
                            pendingId = 0;
                        }

                        bytesToRead--;
                    }
                }
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
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
