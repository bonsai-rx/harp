using Bonsai.Expressions;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Bonsai.Harp
{
    [Description("Parse Harp messages and extracts the data payload.")]
    public class Parse : SelectBuilder
    {
        public Parse()
        {
            Type = PayloadType.U8;
        }

        [Description("The type of payload to parse.")]
        public PayloadType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case PayloadType.U8:
                    return Expression.Call(typeof(Parse), "ProcessU8", null, expression);
                case PayloadType.S8:
                    return Expression.Call(typeof(Parse), "ProcessI8", null, expression);
                case PayloadType.U16:
                    return Expression.Call(typeof(Parse), "ProcessU16", null, expression);
                case PayloadType.S16:
                    return Expression.Call(typeof(Parse), "ProcessI16", null, expression);
                case PayloadType.U32:
                    return Expression.Call(typeof(Parse), "ProcessU32", null, expression);
                case PayloadType.S32:
                    return Expression.Call(typeof(Parse), "ProcessI32", null, expression);
                case PayloadType.U64:
                    return Expression.Call(typeof(Parse), "ProcessU64", null, expression);
                case PayloadType.S64:
                    return Expression.Call(typeof(Parse), "ProcessI64", null, expression);
                case PayloadType.Float:
                    return Expression.Call(typeof(Parse), "ProcessFloat", null, expression);
                case PayloadType.Timestamp:
                    return Expression.Call(typeof(Parse), "ProcessTimestamp", null, expression);
                case PayloadType.TimestampedU8:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedU8", null, expression);
                case PayloadType.TimestampedS8:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedI8", null, expression);
                case PayloadType.TimestampedU16:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedU16", null, expression);
                case PayloadType.TimestampedS16:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedI16", null, expression);
                case PayloadType.TimestampedU32:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedU32", null, expression);
                case PayloadType.TimestampedS32:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedI32", null, expression);
                case PayloadType.TimestampedU64:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedU64", null, expression);
                case PayloadType.TimestampedS64:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedI64", null, expression);
                case PayloadType.TimestampedFloat:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedFloat", null, expression);
                default:
                    throw new InvalidOperationException("Invalid harp data type.");
            }
        }

        static double ParseTimestamp(HarpMessage input)
        {
            if (input.IsTimestamped)
            {
                var seconds = BitConverter.ToUInt32(input.MessageBytes, 5);
                var microseconds = BitConverter.ToUInt16(input.MessageBytes, 5 + 4);
                return seconds + microseconds * 32e-6;
            }
            else
            {
                return 0;
            }
        }

        static bool CheckForErrors(HarpMessage input, PayloadType typeExpected)
        {
            if (input.Error)
            {
                throw new InvalidOperationException("Harp Message has an error.");
            }

            if (input.IsTimestamped)
            {
                if (input.MessageBytes[1] == 10)
                {
                    throw new InvalidOperationException("Harp Message don't have payload.");
                }
            }
            else
            {
                if (input.MessageBytes[1] == 4)
                {
                    throw new InvalidOperationException("Harp Message don't have payload.");
                }
            }

            if ((input.PayloadType & ~PayloadType.Timestamp) != typeExpected)
            {
                throw new InvalidOperationException("Type mismatch.");
            }

            if (!input.IsValid)
            {
                throw new InvalidOperationException("Harp Message is not valid.");
            }

            return false;
        }

        static byte ProcessU8(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.U8);
            return input.IsTimestamped ? input.MessageBytes[11] : input.MessageBytes[5];
        }

        static Timestamped<byte> ProcessTimestampedU8(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.U8);
            var timestamp = ParseTimestamp(input);            
            var value = input.IsTimestamped ? input.MessageBytes[11] : input.MessageBytes[5];
            return new Timestamped<byte>(value, timestamp);
        }

        static sbyte ProcessI8(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.S8);
            return input.IsTimestamped ? (sbyte)input.MessageBytes[11] : (sbyte)input.MessageBytes[5];
        }

        static Timestamped<sbyte> ProcessTimestampedI8(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.S8);
            var timestamp = ParseTimestamp(input);
            var value = input.IsTimestamped ? (sbyte)input.MessageBytes[11] : (sbyte)input.MessageBytes[5];
            return new Timestamped<sbyte>(value, timestamp);
        }

        static ushort ProcessU16(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.U16);
            return input.IsTimestamped ? BitConverter.ToUInt16(input.MessageBytes, 11) : BitConverter.ToUInt16(input.MessageBytes, 5);
        }

        static Timestamped<ushort> ProcessTimestampedU16(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.U16);
            var timestamp = ParseTimestamp(input);
            var value = input.IsTimestamped ? BitConverter.ToUInt16(input.MessageBytes, 11) : BitConverter.ToUInt16(input.MessageBytes, 5);
            return new Timestamped<ushort>(value, timestamp);
        }

        static short ProcessI16(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.S16);
            return input.IsTimestamped ? BitConverter.ToInt16(input.MessageBytes, 11) : BitConverter.ToInt16(input.MessageBytes, 5);
        }

        static Timestamped<short> ProcessTimestampedI16(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.S16);
            var timestamp = ParseTimestamp(input);
            var value = input.IsTimestamped ? BitConverter.ToInt16(input.MessageBytes, 11) : BitConverter.ToInt16(input.MessageBytes, 5);
            return new Timestamped<short>(value, timestamp);
        }

        static uint ProcessU32(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.U32);
            return input.IsTimestamped ? BitConverter.ToUInt32(input.MessageBytes, 11) : BitConverter.ToUInt32(input.MessageBytes, 5);
        }

        static Timestamped<uint> ProcessTimestampedU32(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.U32);
            var timestamp = ParseTimestamp(input);
            var value = input.IsTimestamped ? BitConverter.ToUInt32(input.MessageBytes, 11) : BitConverter.ToUInt32(input.MessageBytes, 5);
            return new Timestamped<uint>(value, timestamp);
        }

        static int ProcessI32(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.S32);
            return input.IsTimestamped ? BitConverter.ToInt32(input.MessageBytes, 11) : BitConverter.ToInt32(input.MessageBytes, 5);
        }

        static Timestamped<int> ProcessTimestampedI32(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.S32);
            var timestamp = ParseTimestamp(input);
            var value = input.IsTimestamped ? BitConverter.ToInt32(input.MessageBytes, 11) : BitConverter.ToInt32(input.MessageBytes, 5);
            return new Timestamped<int>(value, timestamp);
        }

        static ulong ProcessU64(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.U64);
            return input.IsTimestamped ? BitConverter.ToUInt64(input.MessageBytes, 11) : BitConverter.ToUInt64(input.MessageBytes, 5);
        }

        static Timestamped<ulong> ProcessTimestampedU64(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.U64);
            var timestamp = ParseTimestamp(input);
            var value = input.IsTimestamped ? BitConverter.ToUInt64(input.MessageBytes, 11) : BitConverter.ToUInt64(input.MessageBytes, 5);
            return new Timestamped<ulong>(value, timestamp);
        }

        static long ProcessI64(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.S64);
            return input.IsTimestamped ? BitConverter.ToInt64(input.MessageBytes, 11) : BitConverter.ToInt64(input.MessageBytes, 5);
        }

        static Timestamped<long> ProcessTimestampedI64(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.S64);
            var timestamp = ParseTimestamp(input);
            var value = input.IsTimestamped ? BitConverter.ToInt64(input.MessageBytes, 11) : BitConverter.ToInt64(input.MessageBytes, 5);
            return new Timestamped<long>(value, timestamp);
        }

        static float ProcessFloat(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.Float);
            return input.IsTimestamped ? BitConverter.ToSingle(input.MessageBytes, 11): BitConverter.ToSingle(input.MessageBytes, 5);
        }

        static Timestamped<float> ProcessTimestampedFloat(HarpMessage input)
        {
            CheckForErrors(input, PayloadType.Float);
            var timestamp = ParseTimestamp(input);
            var value = input.IsTimestamped ? BitConverter.ToSingle(input.MessageBytes, 11) : BitConverter.ToSingle(input.MessageBytes, 5);
            return new Timestamped<float>(value, timestamp);
        }

        static double ProcessTimestamp(HarpMessage input)
        {
            if (input.Error)
            {
                throw new InvalidOperationException("Harp Message has an error.");
            }

            if (!input.IsTimestamped)
            {
                throw new InvalidOperationException("Harp Message don't have timestamp.");
            }

            if (!input.IsValid)
            {
                throw new InvalidOperationException("Harp Message is not valid.");
            }

            var timestamp = ParseTimestamp(input);
            return timestamp;
        }
    }
}
