﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            this.observer = observer;
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

        static void ProcessThrowException(HarpMessage message)
        {
            if (message.Error)
            {
                string payload;
                bool errorOnType = false;

                try
                {
                    switch ((PayloadType)(message.MessageBytes[4] & ~0x10))
                    {
                        case PayloadType.U8:
                            payload = ((byte)(message.MessageBytes[11])).ToString();
                            break;
                        case PayloadType.S8:
                            payload = ((sbyte)(message.MessageBytes[11])).ToString();
                            break;
                        case PayloadType.U16:
                            payload = (BitConverter.ToUInt16(message.MessageBytes, 11)).ToString();
                            break;
                        case PayloadType.S16:
                            payload = (BitConverter.ToInt16(message.MessageBytes, 11)).ToString();
                            break;
                        case PayloadType.U32:
                            payload = (BitConverter.ToUInt32(message.MessageBytes, 11)).ToString();
                            break;
                        case PayloadType.S32:
                            payload = (BitConverter.ToInt32(message.MessageBytes, 11)).ToString();
                            break;
                        case PayloadType.U64:
                            payload = (BitConverter.ToUInt64(message.MessageBytes, 11)).ToString();
                            break;
                        case PayloadType.S64:
                            payload = (BitConverter.ToInt64(message.MessageBytes, 11)).ToString();
                            break;
                        case PayloadType.Float:
                            payload = (BitConverter.ToSingle(message.MessageBytes, 11)).ToString();
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
                var payloadType = message.PayloadType & ~PayloadType.Timestamp;
                string note = "\n\nNote: If the Payload is an array only the first value is shown here.";
                if (message.MessageType == MessageType.Write)
                {
                    exception = "The device reported an erroneous write command. Check the command details bellow for clues.\nPayload: " + payload + ", Address: " + message.Address + ", Type: " + payloadType + "." + note;
                }
                else
                {
                    if (errorOnType)
                        exception = "The device reported an erroneous read command.\nType not correct for address " + message.Address + ".";
                    else
                        exception = "The device reported an erroneous read command. Check the command details bellow for clues.\nAddress: " + message.Address + ", Type: " + payloadType + "." + note;
                }
                throw new InvalidOperationException(exception);
            }
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
                                if (!IgnoreErrors) ProcessThrowException(dataFrame);
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