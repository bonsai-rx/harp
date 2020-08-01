using Bonsai.Expressions;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Bonsai.Harp
{
    [Description("Extracts the payload data from Harp messages.")]
    public class Parse : SelectBuilder
    {
        public Parse()
        {
            Type = PayloadType.U8;
        }

        [Description("The type of payload data to parse.")]
        public PayloadType Type { get; set; }

        [Description("Indicates whether the payload is an array.")]
        public bool IsArray { get; set; }

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
            return input.GetPayload<TArray>();
        }

        static Timestamped<TArray[]> ProcessTimestampedArray<TArray>(HarpMessage input) where TArray : unmanaged
        {
            var value = input.GetPayload<TArray>(out double timestamp);
            return new Timestamped<TArray[]>(value, timestamp);
        }

        static byte ProcessU8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U8);
            return input.GetPayloadByte();
        }

        static Timestamped<byte> ProcessTimestampedU8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U8);
            var value = input.GetPayloadByte(out double timestamp);
            return new Timestamped<byte>(value, timestamp);
        }

        static sbyte ProcessS8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S8);
            return input.GetPayloadSByte();
        }

        static Timestamped<sbyte> ProcessTimestampedS8(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S8);
            var value = input.GetPayloadSByte(out double timestamp);
            return new Timestamped<sbyte>(value, timestamp);
        }

        static ushort ProcessU16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U16);
            return input.GetPayloadUInt16();
        }

        static Timestamped<ushort> ProcessTimestampedU16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U16);
            var value = input.GetPayloadUInt16(out double timestamp);
            return new Timestamped<ushort>(value, timestamp);
        }

        static short ProcessS16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S16);
            return input.GetPayloadInt16();
        }

        static Timestamped<short> ProcessTimestampedS16(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S16);
            var value = input.GetPayloadInt16(out double timestamp);
            return new Timestamped<short>(value, timestamp);
        }

        static uint ProcessU32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U32);
            return input.GetPayloadUInt32();
        }

        static Timestamped<uint> ProcessTimestampedU32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U32);
            var value = input.GetPayloadUInt32(out double timestamp);
            return new Timestamped<uint>(value, timestamp);
        }

        static int ProcessS32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S32);
            return input.GetPayloadInt32();
        }

        static Timestamped<int> ProcessTimestampedS32(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S32);
            var value = input.GetPayloadInt32(out double timestamp);
            return new Timestamped<int>(value, timestamp);
        }

        static ulong ProcessU64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U64);
            return input.GetPayloadUInt64();
        }

        static Timestamped<ulong> ProcessTimestampedU64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.U64);
            var value = input.GetPayloadUInt64(out double timestamp);
            return new Timestamped<ulong>(value, timestamp);
        }

        static long ProcessS64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S64);
            return input.GetPayloadInt64();
        }

        static Timestamped<long> ProcessTimestampedS64(HarpMessage input)
        {
            CheckErrors(input, PayloadType.S64);
            var value = input.GetPayloadInt64(out double timestamp);
            return new Timestamped<long>(value, timestamp);
        }

        static float ProcessFloat(HarpMessage input)
        {
            CheckErrors(input, PayloadType.Float);
            return input.GetPayloadSingle();
        }

        static Timestamped<float> ProcessTimestampedFloat(HarpMessage input)
        {
            CheckErrors(input, PayloadType.Float);
            var value = input.GetPayloadSingle(out double timestamp);
            return new Timestamped<float>(value, timestamp);
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
