using System;
using System.Runtime.Serialization;

namespace Bonsai.Harp
{
    [Serializable]
    public sealed class HarpException : Exception
    {
        public HarpException()
        {
        }

        public HarpException(string message)
            : base(message)
        {
        }

        public HarpException(string message, Exception inner)
            : base(message, inner)
        {
        }

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
