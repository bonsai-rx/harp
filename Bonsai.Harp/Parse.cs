using Bonsai.Expressions;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which extracts the payload data from an observable sequence of Harp messages.
    /// </summary>
    [Description("Extracts the payload data from Harp messages.")]
    public class Parse : SelectBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parse"/> class.
        /// </summary>
        public Parse()
        {
            Type = PayloadType.U8;
        }

        /// <summary>
        /// Gets or sets the type of payload data to parse.
        /// </summary>
        [Description("The type of payload data to parse.")]
        public PayloadType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payload is an array.
        /// </summary>
        [Description("Indicates whether the payload is an array.")]
        public bool IsArray { get; set; }

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
            var payloadType = Type;
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
                return Expression.Call(typeof(Parse), methodName, new[] { arrayType }, expression);
            }

            switch (payloadType)
            {
                case PayloadType.U8:
                    return Expression.Call(typeof(Parse), nameof(ProcessU8), null, expression);
                case PayloadType.S8:
                    return Expression.Call(typeof(Parse), nameof(ProcessS8), null, expression);
                case PayloadType.U16:
                    return Expression.Call(typeof(Parse), nameof(ProcessU16), null, expression);
                case PayloadType.S16:
                    return Expression.Call(typeof(Parse), nameof(ProcessS16), null, expression);
                case PayloadType.U32:
                    return Expression.Call(typeof(Parse), nameof(ProcessU32), null, expression);
                case PayloadType.S32:
                    return Expression.Call(typeof(Parse), nameof(ProcessS32), null, expression);
                case PayloadType.U64:
                    return Expression.Call(typeof(Parse), nameof(ProcessU64), null, expression);
                case PayloadType.S64:
                    return Expression.Call(typeof(Parse), nameof(ProcessS64), null, expression);
                case PayloadType.Float:
                    return Expression.Call(typeof(Parse), nameof(ProcessFloat), null, expression);
                case PayloadType.Timestamp:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestamp), null, expression);
                case PayloadType.TimestampedU8:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedU8), null, expression);
                case PayloadType.TimestampedS8:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedS8), null, expression);
                case PayloadType.TimestampedU16:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedU16), null, expression);
                case PayloadType.TimestampedS16:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedS16), null, expression);
                case PayloadType.TimestampedU32:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedU32), null, expression);
                case PayloadType.TimestampedS32:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedS32), null, expression);
                case PayloadType.TimestampedU64:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedU64), null, expression);
                case PayloadType.TimestampedS64:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedS64), null, expression);
                case PayloadType.TimestampedFloat:
                    return Expression.Call(typeof(Parse), nameof(ProcessTimestampedFloat), null, expression);
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
            return Timestamped.Create(input.GetPayloadArray<TArray>(out double timestamp), timestamp);
        }

        static byte ProcessU8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U8);
            return input.GetPayloadByte();
        }

        static Timestamped<byte> ProcessTimestampedU8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U8);
            return Timestamped.Create(input.GetPayloadByte(out double timestamp), timestamp);
        }

        static sbyte ProcessS8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S8);
            return input.GetPayloadSByte();
        }

        static Timestamped<sbyte> ProcessTimestampedS8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S8);
            return Timestamped.Create(input.GetPayloadSByte(out double timestamp), timestamp);
        }

        static ushort ProcessU16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U16);
            return input.GetPayloadUInt16();
        }

        static Timestamped<ushort> ProcessTimestampedU16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U16);
            return Timestamped.Create(input.GetPayloadUInt16(out double timestamp), timestamp);
        }

        static short ProcessS16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S16);
            return input.GetPayloadInt16();
        }

        static Timestamped<short> ProcessTimestampedS16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S16);
            return Timestamped.Create(input.GetPayloadInt16(out double timestamp), timestamp);
        }

        static uint ProcessU32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U32);
            return input.GetPayloadUInt32();
        }

        static Timestamped<uint> ProcessTimestampedU32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U32);
            return Timestamped.Create(input.GetPayloadUInt32(out double timestamp), timestamp);
        }

        static int ProcessS32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S32);
            return input.GetPayloadInt32();
        }

        static Timestamped<int> ProcessTimestampedS32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S32);
            return Timestamped.Create(input.GetPayloadInt32(out double timestamp), timestamp);
        }

        static ulong ProcessU64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U64);
            return input.GetPayloadUInt64();
        }

        static Timestamped<ulong> ProcessTimestampedU64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U64);
            return Timestamped.Create(input.GetPayloadUInt64(out double timestamp), timestamp);
        }

        static long ProcessS64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S64);
            return input.GetPayloadInt64();
        }

        static Timestamped<long> ProcessTimestampedS64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S64);
            return Timestamped.Create(input.GetPayloadInt64(out double timestamp), timestamp);
        }

        static float ProcessFloat(HarpMessage input)
        {
            CheckErrors(input, PayloadType.Float);
            return input.GetPayloadSingle();
        }

        static Timestamped<float> ProcessTimestampedFloat(HarpMessage input)
        {
            CheckErrors(input, PayloadType.Float);
            return Timestamped.Create(input.GetPayloadSingle(out double timestamp), timestamp);
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
