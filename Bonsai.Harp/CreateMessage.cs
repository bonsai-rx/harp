using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.Harp
{
    [Description("Creates a new Harp message with the specified payload.")]
    public class CreateMessage : Source<HarpMessage>
    {
        double payload;
        event Action<double> PayloadChanged;

        public CreateMessage()
        {
            Address = 32;
            MessageType = MessageType.Write;
            PayloadType = PayloadType.U8;
            Payload = 0;
        }

        [Description("The type of the Harp message.")]
        public MessageType MessageType { get; set; }

        [Description("The address of the register to which the Harp message refers to.")]
        public int Address { get; set; }

        [Description("The type of data to include in the message payload.")]
        public PayloadType PayloadType { get; set; }

        [Description("The data to write in the message payload.")]
        public double Payload
        {
            get { return payload; }
            set
            {
                this.payload = value;
                PayloadChanged?.Invoke(value);
            }
        }

        HarpMessage GetMessage(double payload)
        {
            var messageType = MessageType;
            var payloadType = PayloadType & ~PayloadType.Timestamp;
            if (messageType == MessageType.Read) return HarpMessage.FromPayload(Address, messageType, payloadType);
            switch (payloadType)
            {
                case PayloadType.U8: return HarpMessage.FromByte(Address, messageType, (byte)payload);
                case PayloadType.S8: return HarpMessage.FromSByte(Address, messageType, (sbyte)payload);
                case PayloadType.U16: return HarpMessage.FromUInt16(Address, messageType, (ushort)payload);
                case PayloadType.S16: return HarpMessage.FromInt16(Address, messageType, (short)payload);
                case PayloadType.U32: return HarpMessage.FromUInt32(Address, messageType, (uint)payload);
                case PayloadType.S32: return HarpMessage.FromInt32(Address, messageType, (int)payload);
                case PayloadType.U64: return HarpMessage.FromUInt64(Address, messageType, (ulong)payload);
                case PayloadType.S64: return HarpMessage.FromInt64(Address, messageType, (long)payload);
                case PayloadType.Float: return HarpMessage.FromSingle(Address, messageType, (float)payload);
                default:
                    throw new InvalidOperationException("Invalid Harp payload type.");
            }
        }

        public override IObservable<HarpMessage> Generate()
        {
            return Observable
                .Defer(() => Observable.Return(GetMessage(payload)))
                .Concat(Observable.FromEvent<double>(
                    handler => PayloadChanged += handler,
                    handler => PayloadChanged -= handler)
                .Select(payload => GetMessage(payload)));
        }

        public IObservable<HarpMessage> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(x => GetMessage(payload));
        }
    }
}
