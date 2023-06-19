using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which formats a sequence of values as
    /// standard Harp device messages.
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
    [Description("Formats a sequence of values as standard Harp device messages.")]
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

        /// <summary>
        /// Gets or sets a value specifying the type of the formatted message.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies the type of the formatted message.")]
        public new MessageType? MessageType
        {
            get { return base.MessageType; }
            set
            {
                base.MessageType = value;
                if (Register is FormatMessagePayload formatMessage)
                {
                    formatMessage.MessageType = value;
                }
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Address
        {
            get => Register is FormatMessagePayload formatMessage ? formatMessage.Address.GetValueOrDefault() : default;
            set { if (Register is FormatMessagePayload formatMessage) formatMessage.Address = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PayloadType PayloadType
        {
            get => Register is FormatMessagePayload formatMessage ? formatMessage.PayloadType.GetValueOrDefault() : default;
            set { if (Register is FormatMessagePayload formatMessage) formatMessage.PayloadType = value; }
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeAddress() => false;

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePayloadType() => false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            if (Register is FormatMessagePayload formatMessage)
            {
                formatMessage.MessageType = MessageType;
                return formatMessage.Build(arguments);
            }
            else return base.Build(arguments);
        }
    }

    /// <summary>
    /// Represents an operator which formats a sequence of values as Harp messages
    /// with the specified address and payload type.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Formats a sequence of values as Harp messages with the specified address and payload type.")]
    public class FormatMessagePayload : SelectBuilder
    {
        /// <summary>
        /// Gets or sets the type of the formatted message.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        [Description("Specifies the type of the formatted message.")]
        public MessageType? MessageType { get; set; } = Harp.MessageType.Write;

        /// <summary>
        /// Gets or sets the address of the register to which the Harp message refers to.
        /// </summary>
        [Description("The address of the register to which the Harp message refers to.")]
        public int? Address { get; set; }

        /// <summary>
        /// Gets or sets the type of data to include in the message payload.
        /// </summary>
        [Description("The type of data to include in the message payload.")]
        public PayloadType? PayloadType { get; set; } = Harp.PayloadType.U8;

        /// <summary>
        /// Returns the expression that specifies how a valid Harp message is created
        /// from the input data.
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
            var baseType = payloadType & ~Harp.PayloadType.Timestamp;
            var timestamped = (payloadType & Harp.PayloadType.Timestamp) == Harp.PayloadType.Timestamp;
            var combinator = Expression.Constant(this, typeof(FormatMessagePayload));
            var address = GetAddressExpression(expression, combinator);
            var messageType = GetMessageTypeExpression(expression, combinator);
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

                var payloadTypeExpression = GetPayloadTypeExpression(expression, combinator);
                if (TryGetArraySegment(expression, out Expression payload))
                {
                    arguments = new[] { address, timestamp, messageType, payloadTypeExpression, payload };
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromPayload), null, arguments);
                }
                arguments = new[] { address, timestamp, messageType, expression };
            }
            else
            {
                var payloadTypeExpression = GetPayloadTypeExpression(expression, combinator);
                if (TryGetArraySegment(expression, out Expression payload))
                {
                    arguments = new[] { address, messageType, payloadTypeExpression, payload };
                    var fromPayload = Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromPayload), null, arguments);
                    if (payloadType == null && expression.Type == typeof(HarpMessage))
                    {
                        var timestamp = Expression.Variable(typeof(double));
                        var isTimestamped = Expression.Call(expression, nameof(HarpMessage.TryGetTimestamp), null, timestamp);
                        arguments = new[] { address, timestamp, messageType, payloadTypeExpression, payload };
                        return Expression.Block(
                            new[] { timestamp },
                            Expression.Condition(
                                isTimestamped,
                                Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromPayload), null, arguments),
                                fromPayload));
                    }
                    else return fromPayload;
                }
                arguments = new[] { address, messageType, expression };
            }

            switch (baseType)
            {
                case Harp.PayloadType.U8:
                    EnsurePayloadType(arguments, expression, typeof(byte));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromByte), null, arguments);
                case Harp.PayloadType.S8:
                    EnsurePayloadType(arguments, expression, typeof(sbyte));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromSByte), null, arguments);
                case Harp.PayloadType.U16:
                    EnsurePayloadType(arguments, expression, typeof(ushort));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromUInt16), null, arguments);
                case Harp.PayloadType.S16:
                    EnsurePayloadType(arguments, expression, typeof(short));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromInt16), null, arguments);
                case Harp.PayloadType.U32:
                    EnsurePayloadType(arguments, expression, typeof(uint));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromUInt32), null, arguments);
                case Harp.PayloadType.S32:
                    EnsurePayloadType(arguments, expression, typeof(int));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromInt32), null, arguments);
                case Harp.PayloadType.U64:
                    EnsurePayloadType(arguments, expression, typeof(ulong));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromUInt64), null, arguments);
                case Harp.PayloadType.S64:
                    EnsurePayloadType(arguments, expression, typeof(long));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromInt64), null, arguments);
                case Harp.PayloadType.Float:
                    EnsurePayloadType(arguments, expression, typeof(float));
                    return Expression.Call(typeof(HarpMessage), nameof(HarpMessage.FromSingle), null, arguments);
                default:
                    throw new InvalidOperationException("Invalid Harp payload type.");
            }
        }

        static bool TryGetArraySegment(Expression expression, out Expression payload)
        {
            if (expression.Type == typeof(HarpMessage))
            {
                payload = Expression.Call(expression, nameof(HarpMessage.GetPayload), null);
                return true;
            }
            else if (expression.Type == typeof(ArraySegment<byte>))
            {
                payload = expression;
                return true;
            }
            else
            {
                payload = null;
                return false;
            }
        }

        static void EnsurePayloadType(Expression[] arguments, Expression payload, Type type)
        {
            if (type != typeof(float)) payload = payload.Type.IsArray ? payload : Expression.Convert(payload, type);
            else payload = payload.Type.IsArray ? payload : Expression.NewArrayInit(type, Expression.Convert(payload, type));
            arguments[arguments.Length - 1] = payload;
        }

        Expression GetAddressExpression(Expression expression, Expression combinator)
        {
            return expression.Type == typeof(HarpMessage)
                ? Expression.Call(combinator, nameof(GetMessageAddress), null, expression)
                : Expression.Call(combinator, nameof(GetMessageAddress), null);
        }

        Expression GetPayloadTypeExpression(Expression expression, Expression combinator)
        {
            return expression.Type == typeof(HarpMessage)
                ? Expression.Call(combinator, nameof(GetMessagePayloadType), null, expression)
                : Expression.Call(combinator, nameof(GetMessagePayloadType), null);
        }

        Expression GetMessageTypeExpression(Expression expression, Expression combinator)
        {
            return expression.Type == typeof(HarpMessage)
                ? Expression.Call(combinator, nameof(GetMessageType), null, expression)
                : Expression.Call(combinator, nameof(GetMessageType), null);
        }

        int GetMessageAddress()
        {
            var address = Address;
            return address.HasValue
                ? address.GetValueOrDefault()
                : throw new InvalidOperationException("No message address is specified.");
        }

        int GetMessageAddress(HarpMessage message)
        {
            var address = Address;
            return address.HasValue ? address.GetValueOrDefault() : message.Address;
        }

        PayloadType GetMessagePayloadType()
        {
            var payloadType = PayloadType;
            return payloadType.HasValue
                ? payloadType.GetValueOrDefault()
                : throw new InvalidOperationException("No message payload type is specified.");
        }

        PayloadType GetMessagePayloadType(HarpMessage message)
        {
            var payloadType = PayloadType;
            return payloadType.HasValue ? payloadType.GetValueOrDefault() : message.PayloadType;
        }

        MessageType GetMessageType()
        {
            var messageType = MessageType;
            return messageType.HasValue
                ? messageType.GetValueOrDefault()
                : throw new InvalidOperationException("No message payload type is specified.");
        }

        MessageType GetMessageType(HarpMessage message)
        {
            var messageType = MessageType;
            return messageType.HasValue ? messageType.GetValueOrDefault() : message.MessageType;
        }
    }
}
