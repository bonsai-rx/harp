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
    /// Represents an operator which filters and selects standard Harp
    /// messages reported by the device.
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
    [Description("Filters and selects standard Harp messages reported by the device.")]
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
            get { return Register is ParseMessagePayload parseMessage && parseMessage.IsArray; }
            set { if (Register is ParseMessagePayload parseMessage) parseMessage.IsArray = value; }
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeType() => false;

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeIsArray() => false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            if (Register is ParseMessagePayload parseMessage)
            {
                return parseMessage.Build(arguments);
            }
            else return base.Build(arguments);
        }
    }

    /// <summary>
    /// Represents an operator which extracts the payload data from an observable sequence of Harp messages.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Extracts the payload data from Harp messages.")]
    public class ParseMessagePayload : SelectBuilder
    {
        readonly FilterRegisterAddress filterMessage = new();

        /// <summary>
        /// Gets or sets the expected message address. This parameter is optional.
        /// </summary>
        [Description("The expected message address. This parameter is optional.")]
        public int? Address
        {
            get => filterMessage.Address;
            set => filterMessage.Address = value;
        }

        /// <summary>
        /// Gets or sets the type of payload data to parse.
        /// </summary>
        [Description("The type of payload data to parse.")]
        public PayloadType PayloadType { get; set; } = PayloadType.U8;

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

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var filter = Expression.Constant(filterMessage);
            return base.Build(arguments.Select(source => Expression.Call(filter, nameof(FilterRegisterAddress.Process), null, source)));
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
                var baseType = payloadType & ~PayloadType.Timestamp;
                var timestamped = (payloadType & PayloadType.Timestamp) == PayloadType.Timestamp;
                var methodName = timestamped ? nameof(ProcessTimestampedArray) : nameof(ProcessArray);
                var arrayType = baseType switch
                {
                    PayloadType.U8 => typeof(byte),
                    PayloadType.S8 => typeof(sbyte),
                    PayloadType.U16 => typeof(ushort),
                    PayloadType.S16 => typeof(short),
                    PayloadType.U32 => typeof(uint),
                    PayloadType.S32 => typeof(int),
                    PayloadType.U64 => typeof(ulong),
                    PayloadType.S64 => typeof(long),
                    PayloadType.Float => typeof(float),
                    _ => throw new InvalidOperationException("Invalid Harp payload array type."),
                };
                return Expression.Call(typeof(ParseMessagePayload), methodName, new[] { arrayType }, expression);
            }

            return payloadType switch
            {
                PayloadType.U8 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessU8), null, expression),
                PayloadType.S8 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessS8), null, expression),
                PayloadType.U16 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessU16), null, expression),
                PayloadType.S16 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessS16), null, expression),
                PayloadType.U32 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessU32), null, expression),
                PayloadType.S32 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessS32), null, expression),
                PayloadType.U64 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessU64), null, expression),
                PayloadType.S64 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessS64), null, expression),
                PayloadType.Float => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessFloat), null, expression),
                PayloadType.Timestamp => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestamp), null, expression),
                PayloadType.TimestampedU8 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedU8), null, expression),
                PayloadType.TimestampedS8 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedS8), null, expression),
                PayloadType.TimestampedU16 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedU16), null, expression),
                PayloadType.TimestampedS16 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedS16), null, expression),
                PayloadType.TimestampedU32 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedU32), null, expression),
                PayloadType.TimestampedS32 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedS32), null, expression),
                PayloadType.TimestampedU64 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedU64), null, expression),
                PayloadType.TimestampedS64 => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedS64), null, expression),
                PayloadType.TimestampedFloat => Expression.Call(typeof(ParseMessagePayload), nameof(ProcessTimestampedFloat), null, expression),
                _ => throw new InvalidOperationException("Invalid Harp payload type."),
            };
        }

        static void CheckErrors(HarpMessage message, PayloadType expectedType)
        {
            if (message.Error)
            {
                throw new ArgumentException("Attempted to parse an error message.", nameof(message));
            }

            var payloadLength = message.GetPayload().Count;
            if (payloadLength == 0)
            {
                throw new ArgumentException("Input message has an empty payload.", nameof(message));
            }

            if ((message.PayloadType & ~PayloadType.Timestamp) != expectedType)
            {
                throw new ArgumentException("Payload type mismatch.", nameof(message));
            }
        }

        static TArray[] ProcessArray<TArray>(HarpMessage message) where TArray : unmanaged
        {
            return message.GetPayloadArray<TArray>();
        }

        static Timestamped<TArray[]> ProcessTimestampedArray<TArray>(HarpMessage input) where TArray : unmanaged
        {
            return input.GetTimestampedPayloadArray<TArray>();
        }

        static byte ProcessU8(HarpMessage message)
        {
            CheckErrors(message, PayloadType.U8);
            return message.GetPayloadByte();
        }

        static Timestamped<byte> ProcessTimestampedU8(HarpMessage message)
        {
            CheckErrors(message, PayloadType.U8);
            return message.GetTimestampedPayloadByte();
        }

        static sbyte ProcessS8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S8);
            return input.GetPayloadSByte();
        }

        static Timestamped<sbyte> ProcessTimestampedS8(HarpMessage message)
        {
            CheckErrors(message, PayloadType.S8);
            return message.GetTimestampedPayloadSByte();
        }

        static ushort ProcessU16(HarpMessage message)
        {
            CheckErrors(message, PayloadType.U16);
            return message.GetPayloadUInt16();
        }

        static Timestamped<ushort> ProcessTimestampedU16(HarpMessage message)
        {
            CheckErrors(message, PayloadType.U16);
            return message.GetTimestampedPayloadUInt16();
        }

        static short ProcessS16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S16);
            return input.GetPayloadInt16();
        }

        static Timestamped<short> ProcessTimestampedS16(HarpMessage message)
        {
            CheckErrors(message, PayloadType.S16);
            return message.GetTimestampedPayloadInt16();
        }

        static uint ProcessU32(HarpMessage message)
        {
            CheckErrors(message, PayloadType.U32);
            return message.GetPayloadUInt32();
        }

        static Timestamped<uint> ProcessTimestampedU32(HarpMessage message)
        {
            CheckErrors(message, PayloadType.U32);
            return message.GetTimestampedPayloadUInt32();
        }

        static int ProcessS32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S32);
            return input.GetPayloadInt32();
        }

        static Timestamped<int> ProcessTimestampedS32(HarpMessage message)
        {
            CheckErrors(message, PayloadType.S32);
            return message.GetTimestampedPayloadInt32();
        }

        static ulong ProcessU64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U64);
            return input.GetPayloadUInt64();
        }

        static Timestamped<ulong> ProcessTimestampedU64(HarpMessage message)
        {
            CheckErrors(message, PayloadType.U64);
            return message.GetTimestampedPayloadUInt64();
        }

        static long ProcessS64(HarpMessage message)
        {
            CheckErrors(message, PayloadType.S64);
            return message.GetPayloadInt64();
        }

        static Timestamped<long> ProcessTimestampedS64(HarpMessage message)
        {
            CheckErrors(message, PayloadType.S64);
            return message.GetTimestampedPayloadInt64();
        }

        static float ProcessFloat(HarpMessage message)
        {
            CheckErrors(message, PayloadType.Float);
            return message.GetPayloadSingle();
        }

        static Timestamped<float> ProcessTimestampedFloat(HarpMessage message)
        {
            CheckErrors(message, PayloadType.Float);
            return message.GetTimestampedPayloadSingle();
        }

        static double ProcessTimestamp(HarpMessage message)
        {
            if (message.Error)
            {
                throw new ArgumentException("Attempted to parse an error message.", nameof(message));
            }

            return message.GetTimestamp();
        }
    }
}
