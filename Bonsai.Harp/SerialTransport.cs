using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    class SerialTransport : IDisposable
    {
        const int DefaultReadBufferSize = 1048576; // 2^20 = 1 MB
        const byte IdMask = 0x03;
        const byte ErrorMask = 0x08;
        readonly IObserver<HarpDataFrame> observer;
        readonly SerialPort serialPort;
        BufferedStream bufferedStream;
        byte[] currentMessage;
        bool ignoreErrors;
        int currentOffset;
        bool notAbleToParse = false;
        int pendingId;
        bool disposed;

        public SerialTransport(string portName, IObserver<HarpDataFrame> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            this.observer = observer;
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

        public bool IgnoreErrors
        {
            get { return ignoreErrors; }
            set { ignoreErrors = value; }
        }

        public void Open()
        {
            serialPort.Open();
        }

        public void Write(HarpDataFrame input)
        {
            serialPort.Write(input.Message, 0, input.Message.Length);
        }

        const byte MaskTypeLength = 0x0F;

        static bool CheckType(byte type)
        {
            switch ((byte)(type & MaskTypeLength))
            {
                case (byte)PayloadType.U8:
                case (byte)PayloadType.U16:
                case (byte)PayloadType.U32:
                case (byte)PayloadType.U64:
                    break;
                default:
                    return false;
            }
            
            if ((type & 0x20) == 0x20)
                return false;

            if ((type & 0x40) == 0x40)
                if ((type & 0xEF) != (byte)PayloadType.Float)
                    return false;

            return true;
        }

        static void ProcessThrowException(HarpDataFrame message)
        {
            if (message.Error)
            {
                string payload;
                bool errorOnType = false;

                try
                {
                    switch ((PayloadType)(message.Message[4] & ~0x10))
                    {
                        case PayloadType.U8:
                            payload = ((byte)(message.Message[11])).ToString();
                            break;
                        case PayloadType.S8:
                            payload = ((sbyte)(message.Message[11])).ToString();
                            break;
                        case PayloadType.U16:
                            payload = (BitConverter.ToUInt16(message.Message, 11)).ToString();
                            break;
                        case PayloadType.S16:
                            payload = (BitConverter.ToInt16(message.Message, 11)).ToString();
                            break;
                        case PayloadType.U32:
                            payload = (BitConverter.ToUInt32(message.Message, 11)).ToString();
                            break;
                        case PayloadType.S32:
                            payload = (BitConverter.ToInt32(message.Message, 11)).ToString();
                            break;
                        case PayloadType.U64:
                            payload = (BitConverter.ToUInt64(message.Message, 11)).ToString();
                            break;
                        case PayloadType.S64:
                            payload = (BitConverter.ToInt64(message.Message, 11)).ToString();
                            break;
                        case PayloadType.Float:
                            payload = (BitConverter.ToSingle(message.Message, 11)).ToString();
                            break;

                        default:
                            payload = "";
                            break;
                    }
                }
                catch (Exception)
                {
                    errorOnType = true;
                    payload = "";
                }


                string exception;

                PayloadType TypeUsedToRead = (PayloadType)(message.Message[4] & ~((byte)(PayloadType.Timestamp)));
                string note = "\n\nNote: If the Payload is an array only the first value is shown here.";

                if (message.Id == MessageId.Write)
                {
                    exception = "The device reported an erroneous write command. Check the command details bellow for clues.\nPayload: " + payload + ", Address: " + message.Address + ", Type: " + TypeUsedToRead + "." + note;
                }
                else
                {
                    if (errorOnType)
                        exception = "The device reported an erroneous read command.\nType not correct for address " + message.Address + ".";
                    else
                        exception = "The device reported an erroneous read command. Check the command details bellow for clues.\nAddress: " + message.Address + ", Type: " + TypeUsedToRead + "." + note;
                }
                throw new InvalidOperationException(exception);
            }
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
                            if (sum == checksum && CheckType(currentMessage[4]))
                            {
                                notAbleToParse = false;

                                var dataFrame = new HarpDataFrame(currentMessage);
                                if (!ignoreErrors) ProcessThrowException(dataFrame);
                                observer.OnNext(dataFrame);
                            }
                            else
                            {
                                var offset = currentMessage.Length - 1;
                                bufferedStream.Seek(-offset);

                                if (!notAbleToParse)
                                {
                                    notAbleToParse = true;

                                    string currentMessageStr = "Not able to parse a Harp Data Frame ";
                                    currentMessageStr += "(" + DateTime.Now.ToString("hh:mm:ss tt", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ")!";
                                    currentMessageStr += "\nRaw Harp Data Frame: ";
                                    currentMessageStr += BitConverter.ToString(currentMessage).Replace("-", ":");
                                    Console.WriteLine(currentMessageStr);
                                }                                                                

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
