using Bonsai;
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
// TODO: replace this with the transform input and output types.
using TResult = System.String;

namespace Bonsai.Harp
{
    public class Parse : SelectBuilder
    {
        public Parse()
        {
            Type = HarpTypes.U8;
        }

        public HarpTypes Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case HarpTypes.U8:
                    return Expression.Call(typeof(Parse), "ProcessU8", null, expression);
                case HarpTypes.I8:
                    return Expression.Call(typeof(Parse), "ProcessI8", null, expression);
                case HarpTypes.U16:
                    return Expression.Call(typeof(Parse), "ProcessU16", null, expression);
                case HarpTypes.I16:
                    return Expression.Call(typeof(Parse), "ProcessI16", null, expression);
                case HarpTypes.U32:
                    return Expression.Call(typeof(Parse), "ProcessU32", null, expression);
                case HarpTypes.I32:
                    return Expression.Call(typeof(Parse), "ProcessI32", null, expression);
                case HarpTypes.U64:
                    return Expression.Call(typeof(Parse), "ProcessU64", null, expression);
                case HarpTypes.I64:
                    return Expression.Call(typeof(Parse), "ProcessI64", null, expression);
                case HarpTypes.Float:
                    return Expression.Call(typeof(Parse), "ProcessFloat", null, expression);
                case HarpTypes.Timestamp | HarpTypes.U8:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedU8", null, expression);
                case HarpTypes.Timestamp | HarpTypes.I8:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedI8", null, expression);
                case HarpTypes.Timestamp | HarpTypes.U16:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedU16", null, expression);
                case HarpTypes.Timestamp | HarpTypes.I16:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedI16", null, expression);
                case HarpTypes.Timestamp | HarpTypes.U32:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedU32", null, expression);
                case HarpTypes.Timestamp | HarpTypes.I32:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedI32", null, expression);
                case HarpTypes.Timestamp | HarpTypes.U64:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedU64", null, expression);
                case HarpTypes.Timestamp | HarpTypes.I64:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedI64", null, expression);
                case HarpTypes.Timestamp | HarpTypes.Float:
                    return Expression.Call(typeof(Parse), "ProcessTimestampedFloat", null, expression);


                case HarpTypes.Timestamp:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException("Invalid harp data type.");
            }
        }

        static double ParseTimestamp(HarpDataFrame input)
        {
            if ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0)
            {
                var seconds = BitConverter.ToUInt32(input.Message, 5);
                var microseconds = BitConverter.ToUInt16(input.Message, 5 + 4);
                return seconds + microseconds * 32e-6;
            }
            else
            {
                return 0;
            }
        }

        static bool CheckForErrors(HarpDataFrame input, HarpType typeExpected)
        {
            if (input.Error)
            {
                throw new InvalidOperationException("Harp Data Frame arrived with error.");
            }

            if ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0)
            {
                if (input.Message[1] == 10)
                    throw new InvalidOperationException("Harp Data Frame arrived without payload.");
            }
            else
            {
                if (input.Message[1] == 4)
                    throw new InvalidOperationException("Harp Data Frame arrived without payload.");
            }

            if ((input.Message[4] & ~(byte)HarpTypes.Timestamp) != (byte)typeExpected)
                throw new InvalidOperationException("Type mismatch.");

            return false;
        }

        static byte ProcessU8(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.U8);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? input.Message[11] : input.Message[5];
        }

        static Timestamped<byte> ProcessTimestampedU8(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.U8);

            var timestamp = ParseTimestamp(input);            
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? input.Message[11] : input.Message[5];
            return new Timestamped<byte>(value, timestamp);
        }

        static sbyte ProcessI8(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.I8);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? (sbyte)input.Message[11] : (sbyte)input.Message[5];
        }

        static Timestamped<sbyte> ProcessTimestampedI8(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.I8);

            var timestamp = ParseTimestamp(input);
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? (sbyte)input.Message[11] : (sbyte)input.Message[5];
            return new Timestamped<sbyte>(value, timestamp);
        }

        static ushort ProcessU16(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.U16);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToUInt16(input.Message, 11) : BitConverter.ToUInt16(input.Message, 5);
        }

        static Timestamped<ushort> ProcessTimestampedU16(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.U16);

            var timestamp = ParseTimestamp(input);
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToUInt16(input.Message, 11) : BitConverter.ToUInt16(input.Message, 5);
            return new Timestamped<ushort>(value, timestamp);
        }

        static short ProcessI16(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.I16);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToInt16(input.Message, 11) : BitConverter.ToInt16(input.Message, 5);
        }

        static Timestamped<short> ProcessTimestampedI16(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.I16);

            var timestamp = ParseTimestamp(input);
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToInt16(input.Message, 11) : BitConverter.ToInt16(input.Message, 5);
            return new Timestamped<short>(value, timestamp);
        }

        static uint ProcessU32(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.U32);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToUInt32(input.Message, 11) : BitConverter.ToUInt32(input.Message, 5);
        }

        static Timestamped<uint> ProcessTimestampedU32(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.U32);

            var timestamp = ParseTimestamp(input);
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToUInt32(input.Message, 11) : BitConverter.ToUInt32(input.Message, 5);
            return new Timestamped<uint>(value, timestamp);
        }

        static int ProcessI32(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.I32);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToInt32(input.Message, 11) : BitConverter.ToInt32(input.Message, 5);
        }

        static Timestamped<int> ProcessTimestampedI32(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.I32);

            var timestamp = ParseTimestamp(input);
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToInt32(input.Message, 11) : BitConverter.ToInt32(input.Message, 5);
            return new Timestamped<int>(value, timestamp);
        }

        static ulong ProcessU64(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.U64);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToUInt64(input.Message, 11) : BitConverter.ToUInt64(input.Message, 5);
        }

        static Timestamped<ulong> ProcessTimestampedU64(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.U64);

            var timestamp = ParseTimestamp(input);
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToUInt64(input.Message, 11) : BitConverter.ToUInt64(input.Message, 5);
            return new Timestamped<ulong>(value, timestamp);
        }

        static long ProcessI64(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.I64);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToInt64(input.Message, 11) : BitConverter.ToInt64(input.Message, 5);
        }

        static Timestamped<long> ProcessTimestampedI64(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.I64);

            var timestamp = ParseTimestamp(input);
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToInt64(input.Message, 11) : BitConverter.ToInt64(input.Message, 5);
            return new Timestamped<long>(value, timestamp);
        }

        static float ProcessFloat(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.Float);

            return ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToSingle(input.Message, 11): BitConverter.ToSingle(input.Message, 5);
        }

        static Timestamped<float> ProcessTimestampedFloat(HarpDataFrame input)
        {
            CheckForErrors(input, HarpType.Float);

            var timestamp = ParseTimestamp(input);
            var value = ((input.Message[4] & (byte)HarpTypes.Timestamp) != 0) ? BitConverter.ToSingle(input.Message, 11) : BitConverter.ToSingle(input.Message, 5);
            return new Timestamped<float>(value, timestamp);
        }
    }
}
