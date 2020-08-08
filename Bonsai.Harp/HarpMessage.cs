using System;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents a single message of the Harp protocol.
    /// </summary>
    public class HarpMessage
    {
        const int BaseOffset = 5;
        const int TimestampedOffset = BaseOffset + 6;
        const int ChecksumSize = 1;
        const int DevicePort = 0xFF;
        const byte ErrorMask = 0x08;

        /// <summary>
        /// Initializes a new instance of the <see cref="HarpMessage"/> class from
        /// the full binary representation of the Harp message.
        /// </summary>
        /// <param name="messageBytes">An array of bytes containing the full binary representation of the Harp message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageBytes"/> is <c>null</c>.
        /// </exception>
        public HarpMessage(params byte[] messageBytes)
        {
            MessageBytes = messageBytes ?? throw new ArgumentNullException(nameof(messageBytes));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarpMessage"/> class from
        /// the full binary representation of the Harp message, and optionally updates
        /// the value of the checksum byte.
        /// </summary>
        /// <param name="updateChecksum">Indicates whether to compute and update the checksum byte.</param>
        /// <param name="messageBytes">An array of bytes containing the full binary representation of the Harp message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageBytes"/> is <c>null</c>.
        /// </exception>
        public HarpMessage(bool updateChecksum, params byte[] messageBytes)
            : this(messageBytes)
        {
            if (updateChecksum)
            {
                messageBytes[messageBytes.Length - 1] = GetChecksum(messageBytes, messageBytes.Length - 1);
            }
        }

        /// <summary>
        /// Gets the type of the Harp message.
        /// </summary>
        public MessageType MessageType
        {
            get { return (MessageType)(MessageBytes[0] & ~ErrorMask); }
        }

        /// <summary>
        /// Gets the address of the register to which the Harp message refers to.
        /// </summary>
        public int Address
        {
            get { return MessageBytes[2]; }
        }

        /// <summary>
        /// If the device is a hub of Harp messages, specifies the origin or destination of the Harp message.
        /// Otherwise, if the message refers to the device itself, returns the default value 0xFF.
        /// </summary>
        public int Port
        {
            get { return MessageBytes[3]; }
        }

        /// <summary>
        /// Gets the type of data available in the message payload.
        /// </summary>
        public PayloadType PayloadType
        {
            get { return (PayloadType)MessageBytes[4]; }
        }

        /// <summary>
        /// Gets a value indicating whether this message is an error report from the device.
        /// </summary>
        public bool Error
        {
            get { return (MessageBytes[0] & ErrorMask) != 0; }
        }

        /// <summary>
        /// Gets the checksum byte used for error detection in the Harp protocol.
        /// </summary>
        public byte Checksum
        {
            get { return MessageBytes[MessageBytes.Length - 1]; }
        }

        /// <summary>
        /// Specifies whether the message payload contains time information.
        /// </summary>
        public bool IsTimestamped
        {
            get { return (PayloadType & PayloadType.Timestamp) == PayloadType.Timestamp; }
        }

        /// <summary>
        /// Specifies whether the message bytes represent a conformant Harp message,
        /// including payload checksum validation.
        /// </summary>
        public bool IsValid
        {
            get
            {
                // validate message type and length fields
                var messageType = MessageType;
                if (messageType < MessageType.Read || messageType > MessageType.Event) return false;
                if (MessageBytes[1] != MessageBytes.Length - 2) return false;

                // validate payload type flags (signed and float cannot both be HIGH, and 0x20 is invalid)
                var payloadType = (int)PayloadType;
                if ((payloadType & 0xC0) == 0xC0 || (payloadType & 0x20) == 0x20) return false;
                
                // base type must be a power of two
                var baseType = payloadType & 0xF;
                if (baseType == 0 || (baseType & (baseType - 1)) != 0) return false;

                // validate checksum
                return Checksum == GetChecksum(MessageBytes, MessageBytes.Length - 1);
            }
        }

        /// <summary>
        /// Gets the full binary representation of the Harp message.
        /// </summary>
        public byte[] MessageBytes { get; private set; }

        private int PayloadOffset
        {
            get { return IsTimestamped ? TimestampedOffset : BaseOffset; }
        }

        /// <summary>
        /// Indicates whether the Harp message matches the specified address.
        /// </summary>
        /// <param name="address">The address to test for a match.</param>
        /// <returns>
        /// <c>true</c> if this Harp message matches the specified address;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(int address)
        {
            return Address == address;
        }

        /// <summary>
        /// Indicates whether the Harp message matches the specified address and message type.
        /// </summary>
        /// <param name="address">The address to test for a match.</param>
        /// <param name="messageType">The message type to test for a match.</param>
        /// <returns>
        /// <c>true</c> if this Harp message matches the specified address and message type;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(int address, MessageType messageType)
        {
            return Address == address && MessageType == messageType;
        }

        /// <summary>
        /// Indicates whether the Harp message matches the specified address and payload type.
        /// </summary>
        /// <param name="address">The address to test for a match.</param>
        /// <param name="payloadType">The payload type to test for a match.</param>
        /// <returns>
        /// <c>true</c> if this Harp message matches the specified address and payload type;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(int address, PayloadType payloadType)
        {
            return Address == address && PayloadType == payloadType;
        }

        /// <summary>
        /// Indicates whether the Harp message matches the specified address, message type,
        /// and payload type.
        /// </summary>
        /// <param name="address">The address to test for a match.</param>
        /// <param name="messageType">The message type to test for a match.</param>
        /// <param name="payloadType">The payload type to test for a match.</param>
        /// <returns>
        /// <c>true</c> if this Harp message matches the specified address, message type,
        /// and payload type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(int address, MessageType messageType, PayloadType payloadType)
        {
            return Address == address && MessageType == messageType && PayloadType == payloadType;
        }

        /// <summary>
        /// Gets the timestamp of the message payload, in seconds.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> representing the timestamp of the message payload, in seconds.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public double GetTimestamp()
        {
            if (!TryGetTimestamp(out double timestamp))
            {
                throw new InvalidOperationException("This Harp message does not have a timestamped payload.");
            }

            return timestamp;
        }

        /// <summary>
        /// Gets the timestamp of the message payload, in seconds. A return value indicates
        /// whether the message has a timestamped payload.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <returns>
        /// <c>true</c> if the message has a timestamped payload; otherwise, <c>false</c>.
        /// </returns>
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
                timestamp = default;
                return false;
            }
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

        int GetPayloadLength(int payloadOffset)
        {
            return MessageBytes.Length - payloadOffset - ChecksumSize;
        }

        void GetPayloadOffset(out int payloadOffset, out int payloadLength)
        {
            payloadOffset = PayloadOffset;
            payloadLength = GetPayloadLength(payloadOffset);
        }

        /// <summary>
        /// Gets the array segment containing the raw message payload. This method
        /// returns a view into the original array without copying any data.
        /// </summary>
        /// <returns>
        /// An <see cref="ArraySegment{T}"/> delimiting the message payload section
        /// of the message bytes.
        /// </returns>
        public ArraySegment<byte> GetPayload()
        {
            GetPayloadOffset(out int payloadOffset, out int payloadLength);
            return new ArraySegment<byte>(MessageBytes, payloadOffset, payloadLength);
        }

        /// <summary>
        /// Gets the array segment containing the raw message payload and the timestamp. This method
        /// returns a view into the original array without copying any data.
        /// </summary>
        /// <returns>
        /// A timestamped <see cref="ArraySegment{T}"/> delimiting the message payload section
        /// of the message bytes.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<ArraySegment<byte>> GetTimestampedPayload()
        {
            var timestamp = GetTimestamp();
            var value = new ArraySegment<byte>(MessageBytes, TimestampedOffset, GetPayloadLength(TimestampedOffset));
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Returns the message payload as an array of non-pointer value types. The size of the
        /// type should be a multiple of the total size of the payload.
        /// </summary>
        /// <typeparam name="TArray">The type of the non-pointer values in the array.</typeparam>
        /// <returns>An array containing a copy of the message payload data.</returns>
        public unsafe TArray[] GetPayloadArray<TArray>() where TArray : unmanaged
        {
            GetPayloadOffset(out int payloadOffset, out int payloadLength);
            var value = new TArray[payloadLength / sizeof(TArray)];
            Buffer.BlockCopy(MessageBytes, payloadOffset, value, 0, payloadLength);
            return value;
        }

        /// <summary>
        /// Returns the message payload as an array of non-pointer value types and gets the message
        /// timestamp. The size of the type should be a multiple of the total size of the payload.
        /// </summary>
        /// <typeparam name="TArray">The type of the non-pointer values in the array.</typeparam>
        /// <returns>A timestamped array containing a copy of the message payload data.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public unsafe Timestamped<TArray[]> GetTimestampedPayloadArray<TArray>() where TArray : unmanaged
        {
            var timestamp = GetTimestamp();
            var payloadLength = GetPayloadLength(TimestampedOffset);
            var value = new TArray[payloadLength / sizeof(TArray)];
            Buffer.BlockCopy(MessageBytes, TimestampedOffset, value, 0, payloadLength);
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single 8-bit unsigned integer.
        /// </summary>
        /// <returns>A <see cref="byte"/> representing the message payload.</returns>
        public byte GetPayloadByte()
        {
            return MessageBytes[PayloadOffset];
        }

        /// <summary>
        /// Returns the message payload as a single 8-bit unsigned integer and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="byte"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<byte> GetTimestampedPayloadByte()
        {
            var timestamp = GetTimestamp();
            return Timestamped.Create(MessageBytes[TimestampedOffset], timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single 8-bit signed integer.
        /// </summary>
        /// <returns>An <see cref="sbyte"/> representing the message payload.</returns>
        public sbyte GetPayloadSByte()
        {
            return (sbyte)MessageBytes[PayloadOffset];
        }

        /// <summary>
        /// Returns the message payload as a single 8-bit signed integer and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="sbyte"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<sbyte> GetTimestampedPayloadSByte()
        {
            var timestamp = GetTimestamp();
            return Timestamped.Create((sbyte)MessageBytes[TimestampedOffset], timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single 16-bit unsigned integer.
        /// </summary>
        /// <returns>A <see cref="ushort"/> representing the message payload.</returns>
        public ushort GetPayloadUInt16()
        {
            return BitConverter.ToUInt16(MessageBytes, PayloadOffset);
        }

        /// <summary>
        /// Returns the message payload as a single 16-bit unsigned integer and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="ushort"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<ushort> GetTimestampedPayloadUInt16()
        {
            var timestamp = GetTimestamp();
            var value = BitConverter.ToUInt16(MessageBytes, TimestampedOffset);
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single 16-bit signed integer.
        /// </summary>
        /// <returns>A <see cref="short"/> representing the message payload.</returns>
        public short GetPayloadInt16()
        {
            return BitConverter.ToInt16(MessageBytes, PayloadOffset);
        }

        /// <summary>
        /// Returns the message payload as a single 16-bit signed integer and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="short"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<short> GetTimestampedPayloadInt16()
        {
            var timestamp = GetTimestamp();
            var value = BitConverter.ToInt16(MessageBytes, TimestampedOffset);
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single 32-bit unsigned integer.
        /// </summary>
        /// <returns>A <see cref="uint"/> representing the message payload.</returns>
        public uint GetPayloadUInt32()
        {
            return BitConverter.ToUInt32(MessageBytes, PayloadOffset);
        }

        /// <summary>
        /// Returns the message payload as a single 32-bit unsigned integer and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="uint"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<uint> GetTimestampedPayloadUInt32()
        {
            var timestamp = GetTimestamp();
            var value = BitConverter.ToUInt32(MessageBytes, TimestampedOffset);
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single 32-bit signed integer.
        /// </summary>
        /// <returns>An <see cref="int"/> representing the message payload.</returns>
        public int GetPayloadInt32()
        {
            return BitConverter.ToInt32(MessageBytes, PayloadOffset);
        }

        /// <summary>
        /// Returns the message payload as a single 32-bit signed integer and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="int"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<int> GetTimestampedPayloadInt32()
        {
            var timestamp = GetTimestamp();
            var value = BitConverter.ToInt32(MessageBytes, TimestampedOffset);
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single 64-bit unsigned integer.
        /// </summary>
        /// <returns>A <see cref="ulong"/> representing the message payload.</returns>
        public ulong GetPayloadUInt64()
        {
            return BitConverter.ToUInt64(MessageBytes, PayloadOffset);
        }

        /// <summary>
        /// Returns the message payload as a single 64-bit unsigned integer and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="ulong"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<ulong> GetTimestampedPayloadUInt64()
        {
            var timestamp = GetTimestamp();
            var value = BitConverter.ToUInt64(MessageBytes, TimestampedOffset);
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single 64-bit signed integer.
        /// </summary>
        /// <returns>A <see cref="long"/> representing the message payload.</returns>
        public long GetPayloadInt64()
        {
            return BitConverter.ToInt64(MessageBytes, PayloadOffset);
        }

        /// <summary>
        /// Returns the message payload as a single 64-bit signed integer and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="long"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<long> GetTimestampedPayloadInt64()
        {
            var timestamp = GetTimestamp();
            var value = BitConverter.ToInt64(MessageBytes, TimestampedOffset);
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Returns the message payload as a single-precision floating point number.
        /// </summary>
        /// <returns>A <see cref="float"/> representing the message payload.</returns>
        public float GetPayloadSingle()
        {
            return BitConverter.ToSingle(MessageBytes, PayloadOffset);
        }

        /// <summary>
        /// Returns the message payload as a single-precision floating point number and gets the message timestamp.
        /// </summary>
        /// <returns>A timestamped <see cref="float"/> representing the message payload.</returns>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public Timestamped<float> GetTimestampedPayloadSingle()
        {
            var timestamp = GetTimestamp();
            var value = BitConverter.ToSingle(MessageBytes, TimestampedOffset);
            return Timestamped.Create(value, timestamp);
        }

        /// <summary>
        /// Copies the message payload into an array of non-pointer value types. The size in bytes of the
        /// array should be equal to or higher than the total size of the payload.
        /// </summary>
        /// <typeparam name="TArray">The type of the non-pointer values in the array.</typeparam>
        /// <param name="value">The array which will contain the copy of the message payload.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of bytes in <paramref name="value"/> is less than the total size of the payload.
        /// </exception>
        public unsafe void CopyTo<TArray>(TArray[] value) where TArray : unmanaged
        {
            CopyTo(value, 0);
        }

        /// <summary>
        /// Copies the message payload into an array of non-pointer value types and gets the message
        /// timestamp. The size in bytes of the array should be equal to or higher than the total
        /// size of the payload.
        /// </summary>
        /// <typeparam name="TArray">The type of the non-pointer values in the array.</typeparam>
        /// <param name="value">The array which will contain the copy of the message payload.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of bytes in <paramref name="value"/> is less than the total size of the payload.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public unsafe void CopyTo<TArray>(TArray[] value, out double timestamp) where TArray : unmanaged
        {
            CopyTo(value, 0, out timestamp);
        }

        /// <summary>
        /// Copies the message payload into an array of non-pointer value types, starting at the specified index.
        /// The size in bytes of the array should be equal to or higher than the total size of the payload.
        /// </summary>
        /// <typeparam name="TArray">The type of the non-pointer values in the array.</typeparam>
        /// <param name="value">The array which will contain the copy of the message payload.</param>
        /// <param name="index">The zero-based offset into <paramref name="value"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of bytes in <paramref name="value"/> is less than <paramref name="index"/> times size of
        /// each array element plus the total size of the payload.
        /// </exception>
        public unsafe void CopyTo<TArray>(TArray[] value, int index) where TArray : unmanaged
        {
            GetPayloadOffset(out int payloadOffset, out int payloadLength);
            Buffer.BlockCopy(MessageBytes, payloadOffset, value, index * sizeof(TArray), payloadLength);
        }

        /// <summary>
        /// Copies the message payload into an array of non-pointer value types, starting at the specified index,
        /// and gets the message timestamp. The size in bytes of the array should be equal to or higher than the total
        /// size of the payload.
        /// </summary>
        /// <typeparam name="TArray">The type of the non-pointer values in the array.</typeparam>
        /// <param name="value">The array which will contain the copy of the message payload.</param>
        /// <param name="index">The zero-based offset into <paramref name="value"/>.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of bytes in <paramref name="value"/> is less than <paramref name="index"/> times size of
        /// each array element plus the total size of the payload.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The message does not have a timestamped payload.
        /// </exception>
        public unsafe void CopyTo<TArray>(TArray[] value, int index, out double timestamp) where TArray : unmanaged
        {
            timestamp = GetTimestamp();
            var payloadLength = GetPayloadLength(TimestampedOffset);
            Buffer.BlockCopy(MessageBytes, TimestampedOffset, value, index * sizeof(TArray), payloadLength);
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

        static HarpMessage FromPayload(int address, int port, MessageType messageType, PayloadType payloadType, Array payload)
        {
            var payloadSize = payload.Length * (0xF & (byte)payloadType);
            var messageBytes = new byte[BaseOffset + payloadSize + ChecksumSize];
            messageBytes[0] = (byte)messageType;
            messageBytes[2] = (byte)address;
            messageBytes[3] = (byte)port;
            messageBytes[4] = (byte)payloadType;
            Buffer.BlockCopy(payload, 0, messageBytes, BaseOffset, payloadSize);
            return FromBytes(messageBytes);
        }

        static HarpMessage FromPayload(int address, int port, double timestamp, MessageType messageType, PayloadType payloadType, Array payload)
        {
            var payloadSize = payload.Length * (0xF & (byte)payloadType);
            var messageBytes = new byte[TimestampedOffset + payloadSize + ChecksumSize];
            messageBytes[0] = (byte)messageType;
            messageBytes[2] = (byte)address;
            messageBytes[3] = (byte)port;
            messageBytes[4] = (byte)payloadType;
            Buffer.BlockCopy(payload, 0, messageBytes, TimestampedOffset, payloadSize);
            return FromBytes(timestamp, messageBytes);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="payloadType">The type of data available in the message payload.</param>
        /// <param name="payload">The raw binary representation of the payload data.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(int address, MessageType messageType, PayloadType payloadType, params byte[] payload)
        {
            return FromPayload(address, DevicePort, messageType, payloadType, payload);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="payloadType">The type of data available in the message payload.</param>
        /// <param name="payload">The raw binary representation of the payload data.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromPayload(int address, int port, MessageType messageType, PayloadType payloadType, params byte[] payload)
        {
            return FromPayload(address, port, messageType, payloadType, (Array)payload);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="payloadType">The type of data available in the message payload.</param>
        /// <param name="payload">The raw binary representation of the payload data.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(int address, double timestamp, MessageType messageType, PayloadType payloadType, params byte[] payload)
        {
            return FromPayload(address, DevicePort, timestamp, messageType, payloadType, payload);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port,
        /// timestamp, and payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="payloadType">The type of data available in the message payload.</param>
        /// <param name="payload">The raw binary representation of the payload data.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(int address, int port, double timestamp, MessageType messageType, PayloadType payloadType, params byte[] payload)
        {
            return FromPayload(address, port, timestamp, messageType, payloadType, (Array)payload);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and a
        /// single value 8-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromByte(int address, MessageType messageType, byte value)
        {
            return FromByte(address, DevicePort, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and a
        /// single value 8-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromByte(int address, int port, MessageType messageType, byte value)
        {
            return FromBytes((byte)messageType, 0, (byte)address, (byte)port, (byte)PayloadType.U8, value, 0);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and a
        /// single value 8-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromByte(int address, double timestamp, MessageType messageType, byte value)
        {
            return FromByte(address, DevicePort, timestamp, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and a
        /// single value 8-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromByte(int address, int port, double timestamp, MessageType messageType, byte value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of 8-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromByte(int address, MessageType messageType, params byte[] values)
        {
            return FromByte(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of 8-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromByte(int address, int port, MessageType messageType, params byte[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.U8, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of 8-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromByte(int address, double timestamp, MessageType messageType, params byte[] values)
        {
            return FromByte(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of 8-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromByte(int address, int port, double timestamp, MessageType messageType, params byte[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedU8, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and a
        /// single value 8-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromSByte(int address, MessageType messageType, sbyte value)
        {
            return FromSByte(address, DevicePort, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and a
        /// single value 8-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromSByte(int address, int port, MessageType messageType, sbyte value)
        {
            return FromBytes((byte)messageType, 0, (byte)address, (byte)port, (byte)PayloadType.U8, (byte)value, 0);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and a
        /// single value 8-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromSByte(int address, double timestamp, MessageType messageType, sbyte value)
        {
            return FromSByte(address, DevicePort, timestamp, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and a
        /// single value 8-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromSByte(int address, int port, double timestamp, MessageType messageType, sbyte value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of 8-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromSByte(int address, MessageType messageType, params sbyte[] values)
        {
            return FromSByte(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of 8-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromSByte(int address, int port, MessageType messageType, params sbyte[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.U8, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of 8-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromSByte(int address, double timestamp, MessageType messageType, params sbyte[] values)
        {
            return FromSByte(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of 8-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromSByte(int address, int port, double timestamp, MessageType messageType, params sbyte[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedU8, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and a
        /// single value 16-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromUInt16(int address, MessageType messageType, ushort value)
        {
            return FromUInt16(address, DevicePort, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and a
        /// single value 16-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromUInt16(int address, int port, MessageType messageType, ushort value)
        {
            return FromBytes((byte)messageType, 0, (byte)address, (byte)port, (byte)PayloadType.U16, (byte)value, (byte)(value >> 8), 0);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and a
        /// single value 16-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt16(int address, double timestamp, MessageType messageType, ushort value)
        {
            return FromUInt16(address, DevicePort, timestamp, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and a
        /// single value 16-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt16(int address, int port, double timestamp, MessageType messageType, ushort value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of 16-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromUInt16(int address, MessageType messageType, params ushort[] values)
        {
            return FromUInt16(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of 16-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromUInt16(int address, int port, MessageType messageType, params ushort[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.U16, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of 16-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt16(int address, double timestamp, MessageType messageType, params ushort[] values)
        {
            return FromUInt16(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of 16-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt16(int address, int port, double timestamp, MessageType messageType, params ushort[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedU16, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and a
        /// single value 16-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromInt16(int address, MessageType messageType, short value)
        {
            return FromInt16(address, DevicePort, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and a
        /// single value 16-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromInt16(int address, int port, MessageType messageType, short value)
        {
            return FromBytes((byte)messageType, 0, (byte)address, (byte)port, (byte)PayloadType.S16, (byte)value, (byte)(value >> 8), 0);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and a
        /// single value 16-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt16(int address, double timestamp, MessageType messageType, short value)
        {
            return FromInt16(address, DevicePort, timestamp, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and a
        /// single value 16-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt16(int address, int port, double timestamp, MessageType messageType, short value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of 16-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromInt16(int address, MessageType messageType, params short[] values)
        {
            return FromInt16(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of 16-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromInt16(int address, int port, MessageType messageType, params short[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.S16, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of 16-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt16(int address, double timestamp, MessageType messageType, params short[] values)
        {
            return FromInt16(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of 16-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt16(int address, int port, double timestamp, MessageType messageType, params short[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedS16, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and a
        /// single value 32-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromUInt32(int address, MessageType messageType, uint value)
        {
            return FromUInt32(address, DevicePort, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and a
        /// single value 32-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromUInt32(int address, int port, MessageType messageType, uint value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and a
        /// single value 32-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt32(int address, double timestamp, MessageType messageType, uint value)
        {
            return FromUInt32(address, DevicePort, timestamp, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and a
        /// single value 32-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt32(int address, int port, double timestamp, MessageType messageType, uint value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of 32-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromUInt32(int address, MessageType messageType, params uint[] values)
        {
            return FromUInt32(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of 32-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromUInt32(int address, int port, MessageType messageType, params uint[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.U32, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of 32-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt32(int address, double timestamp, MessageType messageType, params uint[] values)
        {
            return FromUInt32(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of 32-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt32(int address, int port, double timestamp, MessageType messageType, params uint[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedU32, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and a
        /// single value 32-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromInt32(int address, MessageType messageType, int value)
        {
            return FromInt32(address, DevicePort, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and a
        /// single value 32-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromInt32(int address, int port, MessageType messageType, int value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and a
        /// single value 32-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt32(int address, double timestamp, MessageType messageType, int value)
        {
            return FromInt32(address, DevicePort, timestamp, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and a
        /// single value 32-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt32(int address, int port, double timestamp, MessageType messageType, int value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of 32-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromInt32(int address, MessageType messageType, params int[] values)
        {
            return FromInt32(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of 32-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromInt32(int address, int port, MessageType messageType, params int[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.S32, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of 32-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt32(int address, double timestamp, MessageType messageType, params int[] values)
        {
            return FromInt32(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of 32-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt32(int address, int port, double timestamp, MessageType messageType, params int[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedS32, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and a
        /// single value 64-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromUInt64(int address, MessageType messageType, ulong value)
        {
            return FromUInt64(address, DevicePort, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and a
        /// single value 64-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromUInt64(int address, int port, MessageType messageType, ulong value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and a
        /// single value 64-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt64(int address, double timestamp, MessageType messageType, ulong value)
        {
            return FromUInt64(address, DevicePort, timestamp, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and a
        /// single value 64-bit unsigned integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt64(int address, int port, double timestamp, MessageType messageType, ulong value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of 64-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromUInt64(int address, MessageType messageType, params ulong[] values)
        {
            return FromUInt64(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of 64-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromUInt64(int address, int port, MessageType messageType, params ulong[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.U64, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of 64-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt64(int address, double timestamp, MessageType messageType, params ulong[] values)
        {
            return FromUInt64(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of 64-bit unsigned integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromUInt64(int address, int port, double timestamp, MessageType messageType, params ulong[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedU64, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and a
        /// single value 64-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromInt64(int address, MessageType messageType, long value)
        {
            return FromInt64(address, DevicePort, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and a
        /// single value 64-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromInt64(int address, int port, MessageType messageType, long value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and a
        /// single value 64-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt64(int address, double timestamp, MessageType messageType, long value)
        {
            return FromInt64(address, DevicePort, timestamp, messageType, value);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and a
        /// single value 64-bit signed integer payload.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt64(int address, int port, double timestamp, MessageType messageType, long value)
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

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of 64-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromInt64(int address, MessageType messageType, params long[] values)
        {
            return FromInt64(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of 64-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromInt64(int address, int port, MessageType messageType, params long[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.S64, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of 64-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt64(int address, double timestamp, MessageType messageType, params long[] values)
        {
            return FromInt64(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of 64-bit signed integers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromInt64(int address, int port, double timestamp, MessageType messageType, params long[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedS64, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, and an
        /// array payload of single-precision floating point numbers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type and payload.
        /// </returns>
        public static HarpMessage FromSingle(int address, MessageType messageType, params float[] values)
        {
            return FromSingle(address, DevicePort, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, and an
        /// array payload of single-precision floating point numbers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port, and payload.
        /// </returns>
        public static HarpMessage FromSingle(int address, int port, MessageType messageType, params float[] values)
        {
            return FromPayload(address, port, messageType, PayloadType.Float, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, timestamp, and an
        /// array payload of single-precision floating point numbers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromSingle(int address, double timestamp, MessageType messageType, params float[] values)
        {
            return FromSingle(address, DevicePort, timestamp, messageType, values);
        }

        /// <summary>
        /// Returns a <see cref="HarpMessage"/> with the specified address, message type, port, timestamp, and an
        /// array payload of single-precision floating point numbers.
        /// </summary>
        /// <param name="address">The address of the register to which the Harp message refers to.</param>
        /// <param name="port">The origin or destination of the Harp message, for routing purposes.</param>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="values">The values to be stored in the payload.</param>
        /// <returns>
        /// A valid <see cref="HarpMessage"/> instance with the specified address, message type, port,
        /// timestamp, and payload.
        /// </returns>
        public static HarpMessage FromSingle(int address, int port, double timestamp, MessageType messageType, params float[] values)
        {
            return FromPayload(address, port, timestamp, messageType, PayloadType.TimestampedFloat, values);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the Harp message metadata, payload length and timestamp.
        /// </summary>
        /// <returns>
        /// A formatted <see cref="string"/> representing the Harp message metadata, payload length and timestamp.
        /// </returns>
        public override string ToString()
        {
            if (!IsValid) return "Invalid";
            var payloadType = (byte)PayloadType & 0xF;
            var timestamped = TryGetTimestamp(out double timestamp);
            var payloadLength = GetPayloadLength(timestamped ? TimestampedOffset : BaseOffset) / payloadType;
            return string.Format("{0}{1} {2} {3}{4} Length:{5}",
                Error ? "Error:" : string.Empty,
                MessageType,
                Address,
                PayloadType,
                timestamped ? timestamp.ToString("@0.#####") : string.Empty,
                payloadLength);
        }
    }
}
