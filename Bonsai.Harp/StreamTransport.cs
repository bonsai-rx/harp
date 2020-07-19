using System;
using System.IO;

namespace Bonsai.Harp
{
    class StreamTransport
    {
        const byte IdMask = 0x03;
        const byte ErrorMask = 0x08;
        const byte TypeLengthMask = 0x0F;

        readonly IObserver<HarpMessage> observer;
        BufferedStream bufferedStream;
        byte[] currentMessage;
        bool notAbleToParse;
        int currentOffset;
        int pendingId;

        public StreamTransport(IObserver<HarpMessage> observer)
        {
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
        }

        public bool IgnoreErrors { get; set; }

        static bool CheckType(byte type)
        {
            switch ((byte)(type & TypeLengthMask))
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

        internal void ReceiveData(Stream stream, int readBufferSize, int bytesToRead)
        {
            try
            {
                bufferedStream = bufferedStream ?? new BufferedStream(stream, readBufferSize);
                bufferedStream.PushBytes(bytesToRead);

                while (bytesToRead > 0)
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

                                var dataFrame = new HarpMessage(currentMessage);
                                if (dataFrame.Error && !IgnoreErrors) throw new HarpException(dataFrame);
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
    }
}
