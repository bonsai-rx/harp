using System;
using System.Runtime.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents errors that are reported by Harp devices at runtime.
    /// </summary>
    [Serializable]
    public sealed class HarpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HarpException"/> class.
        /// </summary>
        public HarpException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarpException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HarpException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarpException"/> class with
        /// a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public HarpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HarpException"/> with the specified Harp
        /// error message contents.
        /// </summary>
        /// <param name="message">The Harp message which reported the error.</param>
        public HarpException(HarpMessage message)
            : base(GetErrorMessage(message))
        {
        }

        static string GetErrorMessage(HarpMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var payloadType = message.PayloadType & ~PayloadType.Timestamp;
            if (message.MessageType == MessageType.Write)
            {
                string payload;
                switch (payloadType)
                {
                    case PayloadType.U8: payload = GetPayloadString<byte>(message); break;
                    case PayloadType.S8: payload = GetPayloadString<sbyte>(message); break;
                    case PayloadType.U16: payload = GetPayloadString<ushort>(message); break;
                    case PayloadType.S16: payload = GetPayloadString<short>(message); break;
                    case PayloadType.U32: payload = GetPayloadString<uint>(message); break;
                    case PayloadType.S32: payload = GetPayloadString<int>(message); break;
                    case PayloadType.U64: payload = GetPayloadString<ulong>(message); break;
                    case PayloadType.S64: payload = GetPayloadString<long>(message); break;
                    case PayloadType.Float: payload = GetPayloadString<float>(message); break;
                    default: payload = string.Empty; break;
                }
                return $"The device reported an erroneous write command.\nPayload: {payload}, Address: {message.Address}, Type: {payloadType}.";
            }
            else
            {
                var payload = message.GetPayload();
                if (payload.Count == 0) return $"The device reported an erroneous read command.\nType not correct for address {message.Address}.";
                return $"The device reported an erroneous read command.\nAddress: {message.Address}, Type: {payloadType}.";
            }
        }

        static string GetPayloadString<TArray>(HarpMessage message) where TArray : unmanaged
        {
            const string DataSeparator = ",";
            return string.Join(DataSeparator, message.GetPayload<TArray>());
        }

        private HarpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
