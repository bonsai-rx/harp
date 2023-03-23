using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which creates standard message payloads for
    /// Harp devices.
    /// </summary>
    /// <seealso cref="CreateMessagePayload"/>
    /// <seealso cref="CreateTimestampSecondsPayload"/>
    /// <seealso cref="CreateOperationControlPayload"/>
    /// <seealso cref="CreateResetDevicePayload"/>
    /// <seealso cref="CreateDeviceNamePayload"/>
    /// <seealso cref="CreateSerialNumberPayload"/>
    /// <seealso cref="CreateClockConfigurationPayload"/>
    [XmlInclude(typeof(CreateMessagePayload))]
    [XmlInclude(typeof(CreateTimestampSecondsPayload))]
    [XmlInclude(typeof(CreateOperationControlPayload))]
    [XmlInclude(typeof(CreateResetDevicePayload))]
    [XmlInclude(typeof(CreateDeviceNamePayload))]
    [XmlInclude(typeof(CreateSerialNumberPayload))]
    [XmlInclude(typeof(CreateClockConfigurationPayload))]
    [Description("Creates standard message payloads for Harp devices.")]
    public class CreateMessage : CreateMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessage"/> class.
        /// </summary>
        public CreateMessage()
        {
            Payload = new CreateMessagePayload();
        }

        /// <summary>
        /// Gets or sets the operator used to create specific Harp device message payloads.
        /// </summary>
        [DesignOnly(true)]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The operator used to create specific Harp device message payloads.")]
        [TypeConverter(typeof(CombinatorTypeConverter))]
        public new object Payload
        {
            get { return base.Payload; }
            set
            {
                if (base.Payload is CreateMessagePayload createMessage &&
                    value is XmlNode[] xmlNode && xmlNode.Length == 1)
                {
                    createMessage.Payload = double.Parse(xmlNode[0].InnerText);
                }
                else base.Payload = value;
            }
        }

        string INamedElement.Name => Payload is CreateMessagePayload
            ? default
            : $"Device.{GetElementDisplayName(Payload)}";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MessageType MessageType
        {
            get { return Payload is CreateMessagePayload createMessage ? createMessage.MessageType : default; }
            set { if (Payload is CreateMessagePayload createMessage) createMessage.MessageType = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Address
        {
            get { return Payload is CreateMessagePayload createMessage ? createMessage.Address : default; }
            set { if (Payload is CreateMessagePayload createMessage) createMessage.Address = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PayloadType PayloadType
        {
            get { return Payload is CreateMessagePayload createMessage ? createMessage.PayloadType : default; }
            set { if (Payload is CreateMessagePayload createMessage) createMessage.PayloadType = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMessageType() => false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeAddress() => false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePayloadType() => false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Represents an operator which creates an observable source of Harp messages.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Creates a new Harp message with the specified payload.")]
    public class CreateMessagePayload : Source<HarpMessage>
    {
        double payload;
        event Action<double> PayloadChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMessagePayload"/> class.
        /// </summary>
        public CreateMessagePayload()
        {
            Address = 32;
            MessageType = MessageType.Write;
            PayloadType = PayloadType.U8;
            Payload = 0;
        }

        /// <summary>
        /// Gets or sets the type of the Harp message.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The type of the Harp message.")]
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the address of the register to which the Harp message refers to.
        /// </summary>
        [Description("The address of the register to which the Harp message refers to.")]
        public int Address { get; set; }

        /// <summary>
        /// Gets or sets the type of data to include in the message payload.
        /// </summary>
        [Description("The type of data to include in the message payload.")]
        public PayloadType PayloadType { get; set; }

        /// <summary>
        /// Gets or sets the data to write in the message payload.
        /// </summary>
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

        /// <summary>
        /// Returns an observable sequence that produces a Harp message whenever the payload
        /// property changes, starting with the initial payload value.
        /// </summary>
        /// <returns>
        /// An observable sequence of Harp messages containing the value of the payload property.
        /// </returns>
        public override IObservable<HarpMessage> Generate()
        {
            return Observable
                .Defer(() => Observable.Return(GetMessage(payload)))
                .Concat(Observable.FromEvent<double>(
                    handler => PayloadChanged += handler,
                    handler => PayloadChanged -= handler)
                .Select(payload => GetMessage(payload)));
        }

        /// <summary>
        /// Returns an observable sequence that produces a Harp message with the specified
        /// payload value whenever the source sequence emits a new element.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence used to generate new values.</param>
        /// <returns>
        /// An observable sequence of Harp messages containing the value of the payload property.
        /// </returns>
        public IObservable<HarpMessage> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(x => GetMessage(payload));
        }
    }
}
