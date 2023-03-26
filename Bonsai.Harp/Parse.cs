using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which filters and selects specific messages
    /// reported by the Device device.
    /// </summary>
    /// <seealso cref="ParseMessagePayload"/>
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
    [XmlInclude(typeof(ParseMessagePayload))]
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
    [XmlInclude(typeof(TimestampedWhoAmI))]
    [XmlInclude(typeof(TimestampedHardwareVersionHigh))]
    [XmlInclude(typeof(TimestampedHardwareVersionLow))]
    [XmlInclude(typeof(TimestampedAssemblyVersion))]
    [XmlInclude(typeof(TimestampedCoreVersionHigh))]
    [XmlInclude(typeof(TimestampedCoreVersionLow))]
    [XmlInclude(typeof(TimestampedFirmwareVersionHigh))]
    [XmlInclude(typeof(TimestampedFirmwareVersionLow))]
    [XmlInclude(typeof(TimestampedTimestampSeconds))]
    [XmlInclude(typeof(TimestampedTimestampMicroseconds))]
    [XmlInclude(typeof(TimestampedOperationControl))]
    [XmlInclude(typeof(TimestampedResetDevice))]
    [XmlInclude(typeof(TimestampedDeviceName))]
    [XmlInclude(typeof(TimestampedSerialNumber))]
    [XmlInclude(typeof(TimestampedClockConfiguration))]
    public class Parse : ParseBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Register = new ParseMessagePayload();
        }

        string INamedElement.Name => Register is ParseMessagePayload
            ? default
            : $"Device.{GetElementDisplayName(Register)}";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PayloadType Type
        {
            get { return Register is ParseMessagePayload parseMessage ? parseMessage.PayloadType : default; }
            set { if (Register is ParseMessagePayload parseMessage) parseMessage.PayloadType = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsArray
        {
            get { return Register is ParseMessagePayload parseMessage ? parseMessage.IsArray : default; }
            set { if (Register is ParseMessagePayload parseMessage) parseMessage.IsArray = value; }
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeType() => false;

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeIsArray() => false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Represents an operator which extracts the payload data from an observable sequence of Harp messages.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Extracts the payload data from Harp messages.")]
    public class ParseMessagePayload : SelectBuilder
    {
        readonly FilterMessageAddress filterMessage = new FilterMessageAddress();

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseMessagePayload"/> class.
        /// </summary>
        public ParseMessagePayload()
        {
            PayloadType = PayloadType.U8;
        }

        /// <summary>
        /// Gets or sets the desired message address. This parameter is optional.
        /// </summary>
        [Description("The desired message address. This parameter is optional.")]
        public int? Address
        {
            get => filterMessage.Address;
            set => filterMessage.Address = value;
        }

        /// <summary>
        /// Gets or sets the desired type of the message. This parameter is optional.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The desired type of the message. This parameter is optional.")]
        public MessageType? MessageType
        {
            get => filterMessage.MessageType;
            set => filterMessage.MessageType = value;
        }

        /// <summary>
        /// Gets or sets the type of payload data to parse.
        /// </summary>
        [Description("The type of payload data to parse.")]
        public PayloadType PayloadType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payload is an array.
        /// </summary>
        [Description("Indicates whether the payload is an array.")]
        public bool IsArray { get; set; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Address"/> property
        /// should be serialized.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the <see cref="Address"/> should be serialized;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeAddress() => Address.HasValue;

        /// <summary>
        /// Returns a value indicating whether the <see cref="MessageType"/> property
        /// should be serialized.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the <see cref="MessageType"/> should be serialized;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMessageType() => MessageType.HasValue;

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var filter = Expression.Constant(filterMessage);
            return base.Build(arguments.Select(source => Expression.Call(filter, nameof(FilterMessageAddress.Process), null, source)));
        }

        /// <summary>
        /// Returns the expression that specifies how to extract the payload data from a valid Harp message.
        /// </summary>
        /// <param name="expression">The input parameter to the selector.</param>
        /// <returns>
        /// The <see cref="Expression"/> that maps the input Harp message parameter to the
        /// specified payload data type.
        /// </returns>
        protected override Expression BuildSelector(Expression expression)
        {
            var payloadType = PayloadType;
            if (IsArray && payloadType != PayloadType.Timestamp)
            {
                Type arrayType;
                var baseType = payloadType & ~PayloadType.Timestamp;
                var timestamped = (payloadType & PayloadType.Timestamp) == PayloadType.Timestamp;
                var methodName = timestamped ? nameof(ProcessTimestampedArray) : nameof(ProcessArray);
                switch (baseType)
                {
                    case PayloadType.U8: arrayType = typeof(byte); break;
                    case PayloadType.S8: arrayType = typeof(sbyte); break;
                    case PayloadType.U16: arrayType = typeof(ushort); break;
                    case PayloadType.S16: arrayType = typeof(short); break;
                    case PayloadType.U32: arrayType = typeof(uint); break;
                    case PayloadType.S32: arrayType = typeof(int); break;
                    case PayloadType.U64: arrayType = typeof(ulong); break;
                    case PayloadType.S64: arrayType = typeof(long); break;
                    case PayloadType.Float: arrayType = typeof(float); break;
                    default: throw new InvalidOperationException("Invalid Harp payload array type.");
                }
                return Expression.Call(typeof(ParseMessagePayload), methodName, new[] { arrayType }, expression);
            }

            switch (payloadType)
            {
                case PayloadType.U8:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessU8), null, expression);
                case PayloadType.S8:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessS8), null, expression);
                case PayloadType.U16:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessU16), null, expression);
                case PayloadType.S16:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessS16), null, expression);
                case PayloadType.U32:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessU32), null, expression);
                case PayloadType.S32:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessS32), null, expression);
                case PayloadType.U64:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessU64), null, expression);
                case PayloadType.S64:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessS64), null, expression);
                case PayloadType.Float:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessFloat), null, expression);
                case PayloadType.Timestamp:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestamp), null, expression);
                case PayloadType.TimestampedU8:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedU8), null, expression);
                case PayloadType.TimestampedS8:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedS8), null, expression);
                case PayloadType.TimestampedU16:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedU16), null, expression);
                case PayloadType.TimestampedS16:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedS16), null, expression);
                case PayloadType.TimestampedU32:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedU32), null, expression);
                case PayloadType.TimestampedS32:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedS32), null, expression);
                case PayloadType.TimestampedU64:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedU64), null, expression);
                case PayloadType.TimestampedS64:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedS64), null, expression);
                case PayloadType.TimestampedFloat:
                    return Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedFloat), null, expression);
                default:
                    throw new InvalidOperationException("Invalid Harp payload type.");
            }
        }

        static void CheckErrors(HarpMessage input, PayloadType typeExpected)
        {
            if (input.Error)
            {
                throw new InvalidOperationException("The Harp message is an error report.");
            }

            var payloadLength = input.GetPayload().Count;
            if (payloadLength == 0)
            {
                throw new InvalidOperationException("The Harp message doesn't have a payload.");
            }

            if ((input.PayloadType & ~PayloadType.Timestamp) != typeExpected)
            {
                throw new InvalidOperationException("Payload type mismatch.");
            }
        }

        static TArray[] ProcessArray<TArray>(HarpMessage input) where TArray : unmanaged
        {
            return input.GetPayloadArray<TArray>();
        }

        static Timestamped<TArray[]> ProcessTimestampedArray<TArray>(HarpMessage input) where TArray : unmanaged
        {
            return input.GetTimestampedPayloadArray<TArray>();
        }

        static byte ProcessU8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U8);
            return input.GetPayloadByte();
        }

        static Timestamped<byte> ProcessTimestampedU8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U8);
            return input.GetTimestampedPayloadByte();
        }

        static sbyte ProcessS8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S8);
            return input.GetPayloadSByte();
        }

        static Timestamped<sbyte> ProcessTimestampedS8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S8);
            return input.GetTimestampedPayloadSByte();
        }

        static ushort ProcessU16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U16);
            return input.GetPayloadUInt16();
        }

        static Timestamped<ushort> ProcessTimestampedU16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U16);
            return input.GetTimestampedPayloadUInt16();
        }

        static short ProcessS16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S16);
            return input.GetPayloadInt16();
        }

        static Timestamped<short> ProcessTimestampedS16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S16);
            return input.GetTimestampedPayloadInt16();
        }

        static uint ProcessU32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U32);
            return input.GetPayloadUInt32();
        }

        static Timestamped<uint> ProcessTimestampedU32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U32);
            return input.GetTimestampedPayloadUInt32();
        }

        static int ProcessS32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S32);
            return input.GetPayloadInt32();
        }

        static Timestamped<int> ProcessTimestampedS32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S32);
            return input.GetTimestampedPayloadInt32();
        }

        static ulong ProcessU64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U64);
            return input.GetPayloadUInt64();
        }

        static Timestamped<ulong> ProcessTimestampedU64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U64);
            return input.GetTimestampedPayloadUInt64();
        }

        static long ProcessS64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S64);
            return input.GetPayloadInt64();
        }

        static Timestamped<long> ProcessTimestampedS64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S64);
            return input.GetTimestampedPayloadInt64();
        }

        static float ProcessFloat(HarpMessage input)
        {
            CheckErrors(input, PayloadType.Float);
            return input.GetPayloadSingle();
        }

        static Timestamped<float> ProcessTimestampedFloat(HarpMessage input)
        {
            CheckErrors(input, PayloadType.Float);
            return input.GetTimestampedPayloadSingle();
        }

        static double ProcessTimestamp(HarpMessage input)
        {
            if (input.Error)
            {
                throw new InvalidOperationException("The Harp message is an error report.");
            }

            return input.GetTimestamp();
        }
    }
}
