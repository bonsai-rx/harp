using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    public class HarpMessage
    {
        const int BaseOffset = 5;
        const int TimestampedOffset = BaseOffset + 6;
        const int ChecksumSize = 1;
        const int DevicePort = 0xFF;
        const byte ErrorMask = 0x08;

        public HarpMessage(params byte[] messageBytes)
        {
            if (messageBytes == null)
            {
                throw new ArgumentNullException("messageBytes");
            }

            MessageBytes = messageBytes;
        }

        public HarpMessage(bool updateChecksum, params byte[] messageBytes)
            : this(messageBytes)
        {
            if (updateChecksum)
            {
                messageBytes[messageBytes.Length - 1] = GetChecksum(messageBytes, messageBytes.Length - 1);
            }
        }

        public MessageType MessageType
        {
            get { return (MessageType)(MessageBytes[0] & ~ErrorMask); }
        }

        public int Address
        {
            get { return MessageBytes[2]; }
        }

        public int Port
        {
            get { return MessageBytes[3]; }
        }

        public PayloadType PayloadType
        {
            get { return (PayloadType)MessageBytes[4]; }
        }

        public bool Error
        {
            get { return (MessageBytes[0] & ErrorMask) != 0; }
        }

        public bool IsTimestamped
        {
            get { return (PayloadType & PayloadType.Timestamp) == PayloadType.Timestamp; }
        }

        public bool IsValid
        {
            get
            {
                var messageId = MessageType;
                var payloadType = PayloadType;
                var sizeOfType = (int)payloadType & 0x0F;
                var payloadArrayLength = (MessageBytes.Length - 10) / sizeOfType;

                if ((messageId != MessageType.Write) &&
                    (messageId != MessageType.Read) &&
                    (messageId != MessageType.Event) &&
                    ((byte)messageId != ((byte)MessageType.Write | ErrorMask)) &&
                    ((byte)messageId != ((byte)MessageType.Read | ErrorMask)))
                {
                    return false;
                }

                /* Check if the size of type is correct */
                if ((sizeOfType != 1) && (sizeOfType != 2) && (sizeOfType != 4) && (sizeOfType != 8))
                {
                    return false;
                }

                /* Check if the payload length is an integer number */
                if ((payloadArrayLength % 1) != 0)
                {
                    return false;
                }

                /* Bit 0x20 can't be high */
                if (((int)payloadType & 0x20) == 0x20)
                {
                    return false;
                }
                
                if (GetChecksum() != MessageBytes[MessageBytes.Length - 1])
                {
                    return false;
                }

                return true;
            }
        }

        public byte[] MessageBytes { get; private set; }

        private int PayloadOffset
        {
            get { return IsTimestamped ? TimestampedOffset : BaseOffset; }
        }

        public double GetTimestamp()
        {
            double timestamp;
            if (!TryGetTimestamp(out timestamp))
            {
                throw new InvalidOperationException("This Harp message does not have a timestamped payload.");
            }

            return timestamp;
        }

        public bool TryGetTimestamp(out double timestamp)
        {
            if (IsTimestamped)
            {
                var seconds = BitConverter.ToUInt32(MessageBytes, BaseOffset);
                var microseconds = BitConverter.ToUInt16(MessageBytes, BaseOffset + 4);
                timestamp = seconds + microseconds * 32e-6;
                return true;
            }
            else
            {
                timestamp = default(double);
                return false;
            }
        }

        public byte GetChecksum()
        {
            return GetChecksum(MessageBytes, MessageBytes.Length - 1);
        }

        static byte GetChecksum(byte[] messageBytes, int count)
        {
            var checksum = (byte)0;
            for (int i = 0; i < count; i++)
            {
                checksum += messageBytes[i];
            }
            return checksum;
        }

        int PayloadLength
        {
            get { return MessageBytes.Length - PayloadOffset - ChecksumSize; }
        }

        void GetPayload(Array value, int index)
        {
            Buffer.BlockCopy(MessageBytes, PayloadOffset, value, index, PayloadLength);
        }

        public byte GetPayloadByte()
        {
            return MessageBytes[PayloadOffset];
        }

        public void GetPayloadByte(out byte[] value)
        {
            value = new byte[PayloadLength];
            GetPayloadByte(value);
        }

        public void GetPayloadByte(byte[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadByte(byte[] value, int index)
        {
            GetPayload(value, index);
        }

        public sbyte GetPayloadSByte()
        {
            return (sbyte)MessageBytes[PayloadOffset];
        }

        public void GetPayloadSByte(out sbyte[] value)
        {
            value = new sbyte[PayloadLength];
            GetPayloadSByte(value);
        }

        public void GetPayloadSByte(sbyte[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadSByte(sbyte[] value, int index)
        {
            GetPayload(value, index);
        }

        public ushort GetPayloadUInt16()
        {
            return BitConverter.ToUInt16(MessageBytes, PayloadOffset);
        }

        public void GetPayloadUInt16(out ushort[] value)
        {
            value = new ushort[PayloadLength / sizeof(ushort)];
            GetPayloadUInt16(value);
        }

        public void GetPayloadUInt16(ushort[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadUInt16(ushort[] value, int index)
        {
            GetPayload(value, index * sizeof(ushort));
        }

        public short GetPayloadInt16()
        {
            return BitConverter.ToInt16(MessageBytes, PayloadOffset);
        }

        public void GetPayloadInt16(out short[] value)
        {
            value = new short[PayloadLength / sizeof(short)];
            GetPayloadInt16(value);
        }

        public void GetPayloadInt16(short[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadInt16(short[] value, int index)
        {
            GetPayload(value, index * sizeof(short));
        }

        public uint GetPayloadUInt32()
        {
            return BitConverter.ToUInt32(MessageBytes, PayloadOffset);
        }

        public void GetPayloadUInt32(out uint[] value)
        {
            value = new uint[PayloadLength / sizeof(uint)];
            GetPayloadUInt32(value);
        }

        public void GetPayloadUInt32(uint[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadUInt32(uint[] value, int index)
        {
            GetPayload(value, index * sizeof(uint));
        }

        public int GetPayloadInt32()
        {
            return BitConverter.ToInt32(MessageBytes, PayloadOffset);
        }

        public void GetPayloadInt32(out int[] value)
        {
            value = new int[PayloadLength / sizeof(int)];
            GetPayloadInt32(value);
        }

        public void GetPayloadInt32(int[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadInt32(int[] value, int index)
        {
            GetPayload(value, index * sizeof(int));
        }

        public ulong GetPayloadUInt64()
        {
            return BitConverter.ToUInt64(MessageBytes, PayloadOffset);
        }

        public void GetPayloadUInt64(out ulong[] value)
        {
            value = new ulong[PayloadLength / sizeof(ulong)];
            GetPayloadUInt64(value);
        }

        public void GetPayloadUInt64(ulong[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadUInt64(ulong[] value, int index)
        {
            GetPayload(value, index * sizeof(ulong));
        }

        public long GetPayloadInt64()
        {
            return BitConverter.ToInt64(MessageBytes, PayloadOffset);
        }

        public void GetPayloadInt64(out long[] value)
        {
            value = new long[PayloadLength / sizeof(long)];
            GetPayloadInt64(value);
        }

        public void GetPayloadInt64(long[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadInt64(long[] value, int index)
        {
            GetPayload(value, index * sizeof(long));
        }

        public float GetPayloadSingle()
        {
            return BitConverter.ToSingle(MessageBytes, PayloadOffset);
        }

        public void GetPayloadSingle(out float[] value)
        {
            value = new float[PayloadLength / sizeof(float)];
            GetPayloadSingle(value);
        }

        public void GetPayloadSingle(float[] value)
        {
            GetPayload(value, 0);
        }

        public void GetPayloadSingle(float[] value, int index)
        {
            GetPayload(value, index * sizeof(float));
        }

        static HarpMessage FromBytes(double timestamp, params byte[] messageBytes)
        {
            var seconds = (uint)timestamp;
            var microseconds = (ushort)Math.Round((timestamp - seconds) / 32e-6);
            messageBytes[BaseOffset + 0] = (byte)seconds;
            messageBytes[BaseOffset + 1] = (byte)(seconds >> 8);
            messageBytes[BaseOffset + 2] = (byte)(seconds >> 16);
            messageBytes[BaseOffset + 3] = (byte)(seconds >> 24);
            messageBytes[BaseOffset + 4] = (byte)microseconds;
            messageBytes[BaseOffset + 5] = (byte)(microseconds >> 8);
            return FromBytes(messageBytes);
        }

        static HarpMessage FromBytes(params byte[] messageBytes)
        {
            messageBytes[1] = (byte)(messageBytes.Length - 2);
            messageBytes[messageBytes.Length - 1] = GetChecksum(messageBytes, messageBytes.Length - 1);
            return new HarpMessage(messageBytes);
        }

        static HarpMessage FromArray(MessageType messageType, int address, int port, PayloadType payloadType, Array values)
        {
            var payloadSize = values.Length * (0xF & (byte)payloadType);
            var messageBytes = new byte[BaseOffset + payloadSize + ChecksumSize];
            messageBytes[0] = (byte)messageType;
            messageBytes[2] = (byte)address;
            messageBytes[3] = (byte)port;
            messageBytes[4] = (byte)payloadType;
            Buffer.BlockCopy(values, 0, messageBytes, BaseOffset, payloadSize);
            return FromBytes(messageBytes);
        }

        static HarpMessage FromArray(MessageType messageType, int address, int port, double timestamp, PayloadType payloadType, Array values)
        {
            var payloadSize = values.Length * (0xF & (byte)payloadType);
            var messageBytes = new byte[TimestampedOffset + payloadSize + ChecksumSize];
            messageBytes[0] = (byte)messageType;
            messageBytes[2] = (byte)address;
            messageBytes[3] = (byte)port;
            messageBytes[4] = (byte)payloadType;
            Buffer.BlockCopy(values, 0, messageBytes, TimestampedOffset, payloadSize);
            return FromBytes(timestamp, messageBytes);
        }

        public static HarpMessage FromByte(MessageType messageType, int address, byte value)
        {
            return FromByte(messageType, address, DevicePort, value);
        }

        public static HarpMessage FromByte(MessageType messageType, int address, int port, byte value)
        {
            return FromBytes((byte)messageType, 0, (byte)address, (byte)port, (byte)PayloadType.U8, value, 0);
        }

        public static HarpMessage FromByte(MessageType messageType, int address, double timestamp, byte value)
        {
            return FromByte(messageType, address, DevicePort, timestamp, value);
        }

        public static HarpMessage FromByte(MessageType messageType, int address, int port, double timestamp, byte value)
        {
            return FromBytes(
                timestamp,
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.TimestampedU8,
                0, 0, 0, 0, 0, 0,
                value,
                0);
        }

        public static HarpMessage FromByte(MessageType messageType, int address, params byte[] values)
        {
            return FromByte(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromByte(MessageType messageType, int address, int port, params byte[] values)
        {
            return FromArray(messageType, address, port, PayloadType.U8, values);
        }

        public static HarpMessage FromByte(MessageType messageType, int address, double timestamp, params byte[] values)
        {
            return FromByte(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromByte(MessageType messageType, int address, int port, double timestamp, params byte[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedU8, values);
        }

        public static HarpMessage FromSByte(MessageType messageType, int address, sbyte value)
        {
            return FromSByte(messageType, address, DevicePort, value);
        }

        public static HarpMessage FromSByte(MessageType messageType, int address, int port, sbyte value)
        {
            return FromBytes((byte)messageType, 0, (byte)address, (byte)port, (byte)PayloadType.U8, (byte)value, 0);
        }

        public static HarpMessage FromSByte(MessageType messageType, int address, double timestamp, sbyte value)
        {
            return FromSByte(messageType, address, DevicePort, timestamp, value);
        }

        public static HarpMessage FromSByte(MessageType messageType, int address, int port, double timestamp, sbyte value)
        {
            return FromBytes(
                timestamp,
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.TimestampedU8,
                0, 0, 0, 0, 0, 0,
                (byte)value,
                0);
        }

        public static HarpMessage FromSByte(MessageType messageType, int address, params sbyte[] values)
        {
            return FromSByte(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromSByte(MessageType messageType, int address, int port, params sbyte[] values)
        {
            return FromArray(messageType, address, port, PayloadType.U8, values);
        }

        public static HarpMessage FromSByte(MessageType messageType, int address, double timestamp, params sbyte[] values)
        {
            return FromSByte(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromSByte(MessageType messageType, int address, int port, double timestamp, params sbyte[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedU8, values);
        }

        public static HarpMessage FromUInt16(MessageType messageType, int address, ushort value)
        {
            return FromUInt16(messageType, address, DevicePort, value);
        }

        public static HarpMessage FromUInt16(MessageType messageType, int address, int port, ushort value)
        {
            return FromBytes((byte)messageType, 0, (byte)address, (byte)port, (byte)PayloadType.U16, (byte)value, (byte)(value >> 8), 0);
        }

        public static HarpMessage FromUInt16(MessageType messageType, int address, double timestamp, ushort value)
        {
            return FromUInt16(messageType, address, DevicePort, timestamp, value);
        }

        public static HarpMessage FromUInt16(MessageType messageType, int address, int port, double timestamp, ushort value)
        {
            return FromBytes(
                timestamp,
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.TimestampedU16,
                0, 0, 0, 0, 0, 0,
                (byte)value,
                (byte)(value >> 8),
                0);
        }

        public static HarpMessage FromUInt16(MessageType messageType, int address, params ushort[] values)
        {
            return FromUInt16(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromUInt16(MessageType messageType, int address, int port, params ushort[] values)
        {
            return FromArray(messageType, address, port, PayloadType.U16, values);
        }

        public static HarpMessage FromUInt16(MessageType messageType, int address, double timestamp, params ushort[] values)
        {
            return FromUInt16(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromUInt16(MessageType messageType, int address, int port, double timestamp, params ushort[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedU16, values);
        }

        public static HarpMessage FromInt16(MessageType messageType, int address, short value)
        {
            return FromInt16(messageType, address, DevicePort, value);
        }

        public static HarpMessage FromInt16(MessageType messageType, int address, int port, short value)
        {
            return FromBytes((byte)messageType, 0, (byte)address, (byte)port, (byte)PayloadType.S16, (byte)value, (byte)(value >> 8), 0);
        }

        public static HarpMessage FromInt16(MessageType messageType, int address, double timestamp, short value)
        {
            return FromInt16(messageType, address, DevicePort, timestamp, value);
        }

        public static HarpMessage FromInt16(MessageType messageType, int address, int port, double timestamp, short value)
        {
            return FromBytes(
                timestamp,
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.TimestampedS16,
                0, 0, 0, 0, 0, 0,
                (byte)value,
                (byte)(value >> 8),
                0);
        }

        public static HarpMessage FromInt16(MessageType messageType, int address, params short[] values)
        {
            return FromInt16(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromInt16(MessageType messageType, int address, int port, params short[] values)
        {
            return FromArray(messageType, address, port, PayloadType.S16, values);
        }

        public static HarpMessage FromInt16(MessageType messageType, int address, double timestamp, params short[] values)
        {
            return FromInt16(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromInt16(MessageType messageType, int address, int port, double timestamp, params short[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedS16, values);
        }

        public static HarpMessage FromUInt32(MessageType messageType, int address, uint value)
        {
            return FromUInt32(messageType, address, DevicePort, value);
        }

        public static HarpMessage FromUInt32(MessageType messageType, int address, int port, uint value)
        {
            return FromBytes(
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.U32,
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                0);
        }

        public static HarpMessage FromUInt32(MessageType messageType, int address, double timestamp, uint value)
        {
            return FromUInt32(messageType, address, DevicePort, timestamp, value);
        }

        public static HarpMessage FromUInt32(MessageType messageType, int address, int port, double timestamp, uint value)
        {
            return FromBytes(
                timestamp,
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.TimestampedU32,
                0, 0, 0, 0, 0, 0,
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                0);
        }

        public static HarpMessage FromUInt32(MessageType messageType, int address, params uint[] values)
        {
            return FromUInt32(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromUInt32(MessageType messageType, int address, int port, params uint[] values)
        {
            return FromArray(messageType, address, port, PayloadType.U32, values);
        }

        public static HarpMessage FromUInt32(MessageType messageType, int address, double timestamp, params uint[] values)
        {
            return FromUInt32(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromUInt32(MessageType messageType, int address, int port, double timestamp, params uint[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedU32, values);
        }

        public static HarpMessage FromInt32(MessageType messageType, int address, int value)
        {
            return FromInt32(messageType, address, DevicePort, value);
        }

        public static HarpMessage FromInt32(MessageType messageType, int address, int port, int value)
        {
            return FromBytes(
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.S32,
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                0);
        }

        public static HarpMessage FromInt32(MessageType messageType, int address, double timestamp, int value)
        {
            return FromInt32(messageType, address, DevicePort, timestamp, value);
        }

        public static HarpMessage FromInt32(MessageType messageType, int address, int port, double timestamp, int value)
        {
            return FromBytes(
                timestamp,
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.TimestampedS32,
                0, 0, 0, 0, 0, 0,
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                0);
        }

        public static HarpMessage FromInt32(MessageType messageType, int address, params int[] values)
        {
            return FromInt32(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromInt32(MessageType messageType, int address, int port, params int[] values)
        {
            return FromArray(messageType, address, port, PayloadType.S32, values);
        }

        public static HarpMessage FromInt32(MessageType messageType, int address, double timestamp, params int[] values)
        {
            return FromInt32(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromInt32(MessageType messageType, int address, int port, double timestamp, params int[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedS32, values);
        }

        public static HarpMessage FromUInt64(MessageType messageType, int address, ulong value)
        {
            return FromUInt64(messageType, address, DevicePort, value);
        }

        public static HarpMessage FromUInt64(MessageType messageType, int address, int port, ulong value)
        {
            return FromBytes(
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.U64,
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56),
                0);
        }

        public static HarpMessage FromUInt64(MessageType messageType, int address, double timestamp, ulong value)
        {
            return FromUInt64(messageType, address, DevicePort, timestamp, value);
        }

        public static HarpMessage FromUInt64(MessageType messageType, int address, int port, double timestamp, ulong value)
        {
            return FromBytes(
                timestamp,
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.TimestampedU64,
                0, 0, 0, 0, 0, 0,
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56),
                0);
        }

        public static HarpMessage FromUInt64(MessageType messageType, int address, params ulong[] values)
        {
            return FromUInt64(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromUInt64(MessageType messageType, int address, int port, params ulong[] values)
        {
            return FromArray(messageType, address, port, PayloadType.U64, values);
        }

        public static HarpMessage FromUInt64(MessageType messageType, int address, double timestamp, params ulong[] values)
        {
            return FromUInt64(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromUInt64(MessageType messageType, int address, int port, double timestamp, params ulong[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedU64, values);
        }

        public static HarpMessage FromInt64(MessageType messageType, int address, long value)
        {
            return FromInt64(messageType, address, DevicePort, value);
        }

        public static HarpMessage FromInt64(MessageType messageType, int address, int port, long value)
        {
            return FromBytes(
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.S64,
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56),
                0);
        }

        public static HarpMessage FromInt64(MessageType messageType, int address, double timestamp, long value)
        {
            return FromInt64(messageType, address, DevicePort, timestamp, value);
        }

        public static HarpMessage FromInt64(MessageType messageType, int address, int port, double timestamp, long value)
        {
            return FromBytes(
                timestamp,
                (byte)messageType, 0,
                (byte)address,
                (byte)port,
                (byte)PayloadType.TimestampedS64,
                0, 0, 0, 0, 0, 0,
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56),
                0);
        }

        public static HarpMessage FromInt64(MessageType messageType, int address, params long[] values)
        {
            return FromInt64(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromInt64(MessageType messageType, int address, int port, params long[] values)
        {
            return FromArray(messageType, address, port, PayloadType.S64, values);
        }

        public static HarpMessage FromInt64(MessageType messageType, int address, double timestamp, params long[] values)
        {
            return FromInt64(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromInt64(MessageType messageType, int address, int port, double timestamp, params long[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedS64, values);
        }

        public static HarpMessage FromSingle(MessageType messageType, int address, params float[] values)
        {
            return FromSingle(messageType, address, DevicePort, values);
        }

        public static HarpMessage FromSingle(MessageType messageType, int address, int port, params float[] values)
        {
            return FromArray(messageType, address, port, PayloadType.Float, values);
        }

        public static HarpMessage FromSingle(MessageType messageType, int address, double timestamp, params float[] values)
        {
            return FromSingle(messageType, address, DevicePort, timestamp, values);
        }

        public static HarpMessage FromSingle(MessageType messageType, int address, int port, double timestamp, params float[] values)
        {
            return FromArray(messageType, address, port, timestamp, PayloadType.TimestampedFloat, values);
        }
    }
}
