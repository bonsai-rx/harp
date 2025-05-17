using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which creates a sequence of standard message payloads
    /// for Harp devices.
    /// </summary>
    /// <seealso cref="CreateMessagePayload"/>
    /// <seealso cref="CreateWhoAmIPayload"/>
    /// <seealso cref="CreateHardwareVersionHighPayload"/>
    /// <seealso cref="CreateHardwareVersionLowPayload"/>
    /// <seealso cref="CreateAssemblyVersionPayload"/>
    /// <seealso cref="CreateCoreVersionHighPayload"/>
    /// <seealso cref="CreateCoreVersionLowPayload"/>
    /// <seealso cref="CreateFirmwareVersionHighPayload"/>
    /// <seealso cref="CreateFirmwareVersionLowPayload"/>
    /// <seealso cref="CreateTimestampSecondsPayload"/>
    /// <seealso cref="CreateTimestampMicrosecondsPayload"/>
    /// <seealso cref="CreateOperationControlPayload"/>
    /// <seealso cref="CreateResetDevicePayload"/>
    /// <seealso cref="CreateDeviceNamePayload"/>
    /// <seealso cref="CreateSerialNumberPayload"/>
    /// <seealso cref="CreateClockConfigurationPayload"/>
    [XmlInclude(typeof(CreateMessagePayload))]
    [XmlInclude(typeof(CreateWhoAmIPayload))]
    [XmlInclude(typeof(CreateHardwareVersionHighPayload))]
    [XmlInclude(typeof(CreateHardwareVersionLowPayload))]
    [XmlInclude(typeof(CreateAssemblyVersionPayload))]
    [XmlInclude(typeof(CreateCoreVersionHighPayload))]
    [XmlInclude(typeof(CreateCoreVersionLowPayload))]
    [XmlInclude(typeof(CreateFirmwareVersionHighPayload))]
    [XmlInclude(typeof(CreateFirmwareVersionLowPayload))]
    [XmlInclude(typeof(CreateTimestampSecondsPayload))]
    [XmlInclude(typeof(CreateTimestampMicrosecondsPayload))]
    [XmlInclude(typeof(CreateOperationControlPayload))]
    [XmlInclude(typeof(CreateResetDevicePayload))]
    [XmlInclude(typeof(CreateDeviceNamePayload))]
    [XmlInclude(typeof(CreateSerialNumberPayload))]
    [XmlInclude(typeof(CreateClockConfigurationPayload))]
    [XmlInclude(typeof(CreateTimestampedWhoAmIPayload))]
    [XmlInclude(typeof(CreateTimestampedHardwareVersionHighPayload))]
    [XmlInclude(typeof(CreateTimestampedHardwareVersionLowPayload))]
    [XmlInclude(typeof(CreateTimestampedAssemblyVersionPayload))]
    [XmlInclude(typeof(CreateTimestampedCoreVersionHighPayload))]
    [XmlInclude(typeof(CreateTimestampedCoreVersionLowPayload))]
    [XmlInclude(typeof(CreateTimestampedFirmwareVersionHighPayload))]
    [XmlInclude(typeof(CreateTimestampedFirmwareVersionLowPayload))]
    [XmlInclude(typeof(CreateTimestampedTimestampSecondsPayload))]
    [XmlInclude(typeof(CreateTimestampedTimestampMicrosecondsPayload))]
    [XmlInclude(typeof(CreateTimestampedOperationControlPayload))]
    [XmlInclude(typeof(CreateTimestampedResetDevicePayload))]
    [XmlInclude(typeof(CreateTimestampedDeviceNamePayload))]
    [XmlInclude(typeof(CreateTimestampedSerialNumberPayload))]
    [XmlInclude(typeof(CreateTimestampedClockConfigurationPayload))]
    [Description("Creates a sequence of standard message payloads for Harp devices.")]
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
                    createMessage.Value = double.Parse(xmlNode[0].InnerText);
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

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeAddress() => false;

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePayloadType() => false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Represents an operator which creates a sequence of Harp messages with
    /// the specified payload.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Creates a sequence of Harp messages with the specified payload.")]
    public class CreateMessagePayload
    {
        /// <summary>
        /// Gets or sets the address of the register to which the Harp message refers to.
        /// </summary>
        [Description("The address of the register to which the Harp message refers to.")]
        public int Address { get; set; } = 32;

        /// <summary>
        /// Gets or sets the type of data to include in the message payload.
        /// </summary>
        [Description("The type of data to include in the message payload.")]
        public PayloadType PayloadType { get; set; } = PayloadType.U8;

        /// <summary>
        /// Gets or sets the data to write in the message payload.
        /// </summary>
        [Description("The data to write in the message payload.")]
        public double Value { get; set; }

        /// <summary>
        /// Creates a new Harp message with the specified payload value.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message containing the value of the payload property.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            var payload = Value;
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
        /// Creates a new timestamped Harp message with the specified payload value.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message containing the value of the payload property.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            var payload = Value;
            var payloadType = PayloadType & ~PayloadType.Timestamp;
            if (messageType == MessageType.Read) return HarpMessage.FromPayload(Address, timestamp, messageType, payloadType);
            switch (payloadType)
            {
                case PayloadType.U8: return HarpMessage.FromByte(Address, timestamp, messageType, (byte)payload);
                case PayloadType.S8: return HarpMessage.FromSByte(Address, timestamp, messageType, (sbyte)payload);
                case PayloadType.U16: return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)payload);
                case PayloadType.S16: return HarpMessage.FromInt16(Address, timestamp, messageType, (short)payload);
                case PayloadType.U32: return HarpMessage.FromUInt32(Address, timestamp, messageType, (uint)payload);
                case PayloadType.S32: return HarpMessage.FromInt32(Address, timestamp, messageType, (int)payload);
                case PayloadType.U64: return HarpMessage.FromUInt64(Address, timestamp, messageType, (ulong)payload);
                case PayloadType.S64: return HarpMessage.FromInt64(Address, timestamp, messageType, (long)payload);
                case PayloadType.Float: return HarpMessage.FromSingle(Address, timestamp, messageType, (float)payload);
                default:
                    throw new InvalidOperationException("Invalid Harp payload type.");
            }
        }
    }
}
