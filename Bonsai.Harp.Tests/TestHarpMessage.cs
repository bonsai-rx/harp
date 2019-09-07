using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bonsai.Harp.Tests
{
    [TestClass]
    public class TestHarpMessage
    {
        const int DefaultAddress = 42;
        static readonly Random Generator = new Random(23);

        double GetTimestamp()
        {
            return Math.Round(Generator.NextDouble() * 100 + Generator.NextDouble(), 6);
        }

        void AssertArrayEqual<T>(T[] expected, T[] actual) where T : struct, IEquatable<T>
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].Equals(actual[i]);
            }
        }

        void AssertIsValid(HarpMessage message)
        {
            Assert.IsTrue(message.IsValid);
        }

        void AssertIsValid(HarpMessage message, double timestamp)
        {
            AssertIsValid(message);
            Assert.IsTrue(message.IsTimestamped);
            Assert.AreEqual(timestamp, message.GetTimestamp(), 32e-6);
        }

        void AssertPayload(byte expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadByte());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedByte());
        }

        void AssertPayload(byte[] expected, HarpMessage message)
        {
            byte[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadByte(out payload);
            else message.GetPayloadTimestampedByte(out payload);
            AssertArrayEqual(expected, payload);
        }

        void AssertPayload(sbyte expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadSByte());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedSByte());
        }

        void AssertPayload(sbyte[] expected, HarpMessage message)
        {
            sbyte[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadSByte(out payload);
            else message.GetPayloadTimestampedSByte(out payload);
            AssertArrayEqual(expected, payload);
        }

        void AssertPayload(ushort expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadUInt16());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedUInt16());
        }

        void AssertPayload(ushort[] expected, HarpMessage message)
        {
            ushort[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadUInt16(out payload);
            else message.GetPayloadTimestampedUInt16(out payload);
            AssertArrayEqual(expected, payload);
        }

        void AssertPayload(short expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadInt16());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedInt16());
        }

        void AssertPayload(short[] expected, HarpMessage message)
        {
            short[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadInt16(out payload);
            else message.GetPayloadTimestampedInt16(out payload);
            AssertArrayEqual(expected, payload);
        }

        void AssertPayload(uint expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadUInt32());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedUInt32());
        }

        void AssertPayload(uint[] expected, HarpMessage message)
        {
            uint[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadUInt32(out payload);
            else message.GetPayloadTimestampedUInt32(out payload);
            AssertArrayEqual(expected, payload);
        }

        void AssertPayload(int expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadInt32());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedInt32());
        }

        void AssertPayload(int[] expected, HarpMessage message)
        {
            int[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadInt32(out payload);
            else message.GetPayloadTimestampedInt32(out payload);
            AssertArrayEqual(expected, payload);
        }

        void AssertPayload(ulong expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadUInt64());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedUInt64());
        }

        void AssertPayload(ulong[] expected, HarpMessage message)
        {
            ulong[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadUInt64(out payload);
            else message.GetPayloadTimestampedUInt64(out payload);
            AssertArrayEqual(expected, payload);
        }

        void AssertPayload(long expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadInt64());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedInt64());
        }

        void AssertPayload(long[] expected, HarpMessage message)
        {
            long[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadInt64(out payload);
            else message.GetPayloadTimestampedInt64(out payload);
            AssertArrayEqual(expected, payload);
        }

        void AssertPayload(float expected, HarpMessage message)
        {
            AssertIsValid(message);
            if (!message.IsTimestamped) Assert.AreEqual(expected, message.GetPayloadSingle());
            else Assert.AreEqual(expected, message.GetPayloadTimestampedSingle());
        }

        void AssertPayload(float[] expected, HarpMessage message)
        {
            float[] payload;
            AssertIsValid(message);
            if (!message.IsTimestamped) message.GetPayloadSingle(out payload);
            else message.GetPayloadTimestampedSingle(out payload);
            AssertArrayEqual(expected, payload);
        }

        [TestMethod]
        public void FromByte_Value_PayloadHasValue()
        {
            byte value = 23;
            var message = HarpMessage.FromByte(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromByte_Array_PayloadHasValue()
        {
            var value = new byte[] { 23, 17 };
            var message = HarpMessage.FromByte(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromSByte_Value_PayloadHasValue()
        {
            sbyte value = -3;
            var message = HarpMessage.FromSByte(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromSByte_Array_PayloadHasValue()
        {
            var value = new sbyte[] { 4, -23 };
            var message = HarpMessage.FromSByte(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt16_Value_PayloadHasValue()
        {
            ushort value = 1024;
            var message = HarpMessage.FromUInt16(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt16_Array_PayloadHasValue()
        {
            var value = new ushort[] { 512, 2048 };
            var message = HarpMessage.FromUInt16(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt16_Value_PayloadHasValue()
        {
            short value = 1024;
            var message = HarpMessage.FromInt16(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt16_Array_PayloadHasValue()
        {
            var value = new short[] { 512, -2048 };
            var message = HarpMessage.FromInt16(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt32_Value_PayloadHasValue()
        {
            uint value = 123456789;
            var message = HarpMessage.FromUInt32(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt32_Array_PayloadHasValue()
        {
            var value = new uint[] { uint.MaxValue / 2, 123456789 };
            var message = HarpMessage.FromUInt32(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt32_Value_PayloadHasValue()
        {
            int value = -123456789;
            var message = HarpMessage.FromInt32(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt32_Array_PayloadHasValue()
        {
            var value = new int[] { int.MaxValue / 2, -123456789 };
            var message = HarpMessage.FromInt32(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt64_Value_PayloadHasValue()
        {
            ulong value = int.MaxValue * 2u;
            var message = HarpMessage.FromUInt64(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt64_Array_PayloadHasValue()
        {
            var value = new ulong[] { int.MaxValue * 2u, 123456789 };
            var message = HarpMessage.FromUInt64(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt64_Value_PayloadHasValue()
        {
            long value = -int.MaxValue * 2u;
            var message = HarpMessage.FromInt64(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt64_Array_PayloadHasValue()
        {
            var value = new long[] { int.MaxValue * -2u, 123456789 };
            var message = HarpMessage.FromInt64(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromSingle_Value_PayloadHasValue()
        {
            float value = (float)Math.E;
            var message = HarpMessage.FromSingle(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromSingle_Array_PayloadHasValue()
        {
            var value = new float[] { (float)Math.PI, 1 / 3f };
            var message = HarpMessage.FromSingle(MessageType.Write, DefaultAddress, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromByte_TimestampedValue_PayloadHasValue()
        {
            byte value = 23;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromByte(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromByte_TimestampedArray_PayloadHasValue()
        {
            var value = new byte[] { 23, 17 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromByte(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromSByte_TimestampedValue_PayloadHasValue()
        {
            sbyte value = -3;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromSByte(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromSByte_TimestampedArray_PayloadHasValue()
        {
            var value = new sbyte[] { 4, -23 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromSByte(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt16_TimestampedValue_PayloadHasValue()
        {
            ushort value = 1024;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt16(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt16_TimestampedArray_PayloadHasValue()
        {
            var value = new ushort[] { 512, 2048 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt16(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt16_TimestampedValue_PayloadHasValue()
        {
            short value = 1024;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt16(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt16_TimestampedArray_PayloadHasValue()
        {
            var value = new short[] { 512, -2048 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt16(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt32_TimestampedValue_PayloadHasValue()
        {
            uint value = 123456789;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt32(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt32_TimestampedArray_PayloadHasValue()
        {
            var value = new uint[] { uint.MaxValue / 2, 123456789 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt32(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt32_TimestampedValue_PayloadHasValue()
        {
            int value = -123456789;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt32(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt32_TimestampedArray_PayloadHasValue()
        {
            var value = new int[] { int.MaxValue / 2, -123456789 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt32(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt64_TimestampedValue_PayloadHasValue()
        {
            ulong value = int.MaxValue * 2u;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt64(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromUInt64_TimestampedArray_PayloadHasValue()
        {
            var value = new ulong[] { int.MaxValue * 2u, 123456789 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt64(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt64_TimestampedValue_PayloadHasValue()
        {
            long value = -int.MaxValue * 2u;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt64(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromInt64_TimestampedArray_PayloadHasValue()
        {
            var value = new long[] { int.MaxValue * -2u, 123456789 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt64(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromSingle_TimestampedValue_PayloadHasValue()
        {
            float value = (float)Math.E;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromSingle(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }

        [TestMethod]
        public void FromSingle_TimestampedArray_PayloadHasValue()
        {
            var value = new float[] { (float)Math.PI, 1 / 3f };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromSingle(MessageType.Write, DefaultAddress, timestamp, value);
            AssertPayload(value, message);
        }
    }
}
