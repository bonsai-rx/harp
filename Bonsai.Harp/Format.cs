using Bonsai.Expressions;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which formats a sequence of values as specific
    /// Harp device register messages.
    /// </summary>
    /// <seealso cref="FormatMessagePayload"/>
    /// <seealso cref="WhoAmI"/>
    /// <seealso cref="HardwareVersionHigh"/>
    /// <seealso cref="HardwareVersionLow"/>
    /// <seealso cref="AssemblyVersion"/>
    /// <seealso cref="CoreVersionHigh"/>
    /// <seealso cref="CoreVersionLow"/>
    /// <seealso cref="FirmwareVersionHigh"/>
    /// <seealso cref="FirmwareVersionLow"/>
    /// <seealso cref="TimestampSeconds"/>
    /// <seealso cref="TimestampMicroseconds"/>
    /// <seealso cref="OperationControl"/>
    /// <seealso cref="ResetDevice"/>
    /// <seealso cref="DeviceName"/>
    /// <seealso cref="SerialNumber"/>
    /// <seealso cref="ClockConfiguration"/>
    [XmlInclude(typeof(FormatMessagePayload))]
    [XmlInclude(typeof(WhoAmI))]
    [XmlInclude(typeof(HardwareVersionHigh))]
    [XmlInclude(typeof(HardwareVersionLow))]
    [XmlInclude(typeof(AssemblyVersion))]
    [XmlInclude(typeof(CoreVersionHigh))]
    [XmlInclude(typeof(CoreVersionLow))]
    [XmlInclude(typeof(FirmwareVersionHigh))]
    [XmlInclude(typeof(FirmwareVersionLow))]
    [XmlInclude(typeof(TimestampSeconds))]
    [XmlInclude(typeof(TimestampMicroseconds))]
    [XmlInclude(typeof(OperationControl))]
    [XmlInclude(typeof(ResetDevice))]
    [XmlInclude(typeof(DeviceName))]
    [XmlInclude(typeof(SerialNumber))]
    [XmlInclude(typeof(ClockConfiguration))]
    [Description("Formats a sequence of values as specific Device register messages.")]
    public class Format : FormatBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Format"/> class.
        /// </summary>
        public Format()
        {
            Register = new FormatMessagePayload();
        }

        string INamedElement.Name => Register is FormatMessagePayload
            ? default
            : $"Device.{GetElementDisplayName(Register)}";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MessageType MessageType
        {
            get { return Register is FormatMessagePayload formatMessage ? formatMessage.MessageType : default; }
            set { if (Register is FormatMessagePayload formatMessage) formatMessage.MessageType = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Address
        {
            get { return Register is FormatMessagePayload formatMessage ? formatMessage.Address : default; }
            set { if (Register is FormatMessagePayload formatMessage) formatMessage.Address = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PayloadType PayloadType
        {
            get { return Register is FormatMessagePayload formatMessage ? formatMessage.PayloadType : default; }
            set { if (Register is FormatMessagePayload formatMessage) formatMessage.PayloadType = value; }
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
    /// Represents an operator which formats input data as a Harp message payload.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Formats input data as a Harp message.")]
    public class FormatMessagePayload : SelectBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatMessagePayload"/> class.
        /// </summary>
        public FormatMessagePayload()
        {
            Address = 32;
            MessageType = MessageType.Write;
            PayloadType = PayloadType.U8;
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
        /// Returns the expression that specifies how a valid Harp message is created from the input data.
        /// </summary>
        /// <param name="expression">The input parameter to the selector.</param>
        /// <returns>
        /// The <see cref="Expression"/> that maps the input parameter to the
        /// valid Harp message.
        /// </returns>
        protected override Expression BuildSelector(Expression expression)
        {
            Expression[] arguments;
            var payloadType = PayloadType;
            var baseType = payloadType & ~PayloadType.Timestamp;
            var timestamped = (payloadType & PayloadType.Timestamp) == PayloadType.Timestamp;
            var address = Expression.Constant(Address);
            var messageType = Expression.Constant(MessageType);
            if (timestamped)
            {
                Expression timestamp;
                if (expression.Type == typeof(HarpMessage))
                {
                    timestamp = Expression.Call(expression, nameof(HarpMessage.GetTimestamp), null);
                }
                else
                {
                    timestamp = Expression.PropertyOrField(expression, nameof(Timestamped<object>.Seconds));
                    if (timestamp.Type != typeof(double)) timestamp = Expression.Convert(timestamp, typeof(double));
                    expression = Expression.PropertyOrField(expression, nameof(Timestamped<object>.Value));
                }

                if (TryGetArraySegment(ref expression))
                {
                    arguments = new[] { address, timestamp, messageType, Expression.Constant(payloadType), expression };
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromPayload), null, arguments);
                }
                arguments = new[] { address, timestamp, messageType, expression };
            }
            else
            {
                if (TryGetArraySegment(ref expression))
                {
                    arguments = new[] { address, messageType, Expression.Constant(payloadType), expression };
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromPayload), null, arguments);
                }
                arguments = new[] { address, messageType, expression };
            }

            switch (baseType)
            {
                case PayloadType.U8:
                    EnsurePayloadType(arguments, expression, typeof(byte));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromByte), null, arguments);
                case PayloadType.S8:
                    EnsurePayloadType(arguments, expression, typeof(sbyte));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromSByte), null, arguments);
                case PayloadType.U16:
                    EnsurePayloadType(arguments, expression, typeof(ushort));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromUInt16), null, arguments);
                case PayloadType.S16:
                    EnsurePayloadType(arguments, expression, typeof(short));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromInt16), null, arguments);
                case PayloadType.U32:
                    EnsurePayloadType(arguments, expression, typeof(uint));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromUInt32), null, arguments);
                case PayloadType.S32:
                    EnsurePayloadType(arguments, expression, typeof(int));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromInt32), null, arguments);
                case PayloadType.U64:
                    EnsurePayloadType(arguments, expression, typeof(ulong));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromUInt64), null, arguments);
                case PayloadType.S64:
                    EnsurePayloadType(arguments, expression, typeof(long));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromInt64), null, arguments);
                case PayloadType.Float:
                    EnsurePayloadType(arguments, expression, typeof(float));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromSingle), null, arguments);
                default:
                    throw new InvalidOperationException("Invalid Harp payload type.");
            }
        }

        static bool TryGetArraySegment(ref Expression expression)
        {
            if (expression.Type == typeof(HarpMessage))
            {
                expression = Expression.Call(expression, nameof(HarpMessage.GetPayload), null);
                return true;
            }
            return expression.Type == typeof(ArraySegment<byte>);
        }

        static void EnsurePayloadType(Expression[] arguments, Expression payload, Type type)
        {
            if (type != typeof(float)) payload = payload.Type.IsArray ? payload : Expression.Convert(payload, type);
            else payload = payload.Type.IsArray? payload : Expression.NewArrayInit(type, Expression.Convert(payload, type));
            arguments[arguments.Length - 1] = payload;
        }
    }
}
