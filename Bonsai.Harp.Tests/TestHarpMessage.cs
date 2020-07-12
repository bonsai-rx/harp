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

        void AssertTimestamp(double expected, double actual)
        {
            Assert.AreEqual(expected, actual, 32e-6);
        }

        [TestMethod]
        public void FromByte_Value_PayloadHasValue()
        {
            byte value = 23;
            var message = HarpMessage.FromByte(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadByte());
        }

        [TestMethod]
        public void FromByte_Array_PayloadHasValue()
        {
            var value = new byte[] { 23, 17 };
            var message = HarpMessage.FromByte(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<byte>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromSByte_Value_PayloadHasValue()
        {
            sbyte value = -3;
            var message = HarpMessage.FromSByte(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadSByte());
        }

        [TestMethod]
        public void FromSByte_Array_PayloadHasValue()
        {
            var value = new sbyte[] { 4, -23 };
            var message = HarpMessage.FromSByte(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<sbyte>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt16_Value_PayloadHasValue()
        {
            ushort value = 1024;
            var message = HarpMessage.FromUInt16(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadUInt16());
        }

        [TestMethod]
        public void FromUInt16_Array_PayloadHasValue()
        {
            var value = new ushort[] { 512, 2048 };
            var message = HarpMessage.FromUInt16(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<ushort>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromInt16_Value_PayloadHasValue()
        {
            short value = 1024;
            var message = HarpMessage.FromInt16(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadInt16());
        }

        [TestMethod]
        public void FromInt16_Array_PayloadHasValue()
        {
            var value = new short[] { 512, -2048 };
            var message = HarpMessage.FromInt16(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<short>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt32_Value_PayloadHasValue()
        {
            uint value = 123456789;
            var message = HarpMessage.FromUInt32(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadUInt32());
        }

        [TestMethod]
        public void FromUInt32_Array_PayloadHasValue()
        {
            var value = new uint[] { uint.MaxValue / 2, 123456789 };
            var message = HarpMessage.FromUInt32(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<uint>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromInt32_Value_PayloadHasValue()
        {
            int value = -123456789;
            var message = HarpMessage.FromInt32(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadInt32());
        }

        [TestMethod]
        public void FromInt32_Array_PayloadHasValue()
        {
            var value = new int[] { int.MaxValue / 2, -123456789 };
            var message = HarpMessage.FromInt32(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<int>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt64_Value_PayloadHasValue()
        {
            ulong value = int.MaxValue * 2u;
            var message = HarpMessage.FromUInt64(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadUInt64());
        }

        [TestMethod]
        public void FromUInt64_Array_PayloadHasValue()
        {
            var value = new ulong[] { int.MaxValue * 2u, 123456789 };
            var message = HarpMessage.FromUInt64(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<ulong>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromInt64_Value_PayloadHasValue()
        {
            long value = -int.MaxValue * 2u;
            var message = HarpMessage.FromInt64(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadInt64());
        }

        [TestMethod]
        public void FromInt64_Array_PayloadHasValue()
        {
            var value = new long[] { int.MaxValue * -2u, 123456789 };
            var message = HarpMessage.FromInt64(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<long>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromSingle_Value_PayloadHasValue()
        {
            float value = (float)Math.E;
            var message = HarpMessage.FromSingle(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            Assert.AreEqual(value, message.GetPayloadSingle());
        }

        [TestMethod]
        public void FromSingle_Array_PayloadHasValue()
        {
            var value = new float[] { (float)Math.PI, 1 / 3f };
            var message = HarpMessage.FromSingle(DefaultAddress, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<float>();
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromByte_TimestampedValue_PayloadHasValue()
        {
            byte value = 23;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromByte(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadByte(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromByte_TimestampedArray_PayloadHasValue()
        {
            var value = new byte[] { 23, 17 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromByte(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<byte>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromSByte_TimestampedValue_PayloadHasValue()
        {
            sbyte value = -3;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromSByte(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadSByte(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromSByte_TimestampedArray_PayloadHasValue()
        {
            var value = new sbyte[] { 4, -23 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromSByte(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<sbyte>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt16_TimestampedValue_PayloadHasValue()
        {
            ushort value = 1024;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt16(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadUInt16(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt16_TimestampedArray_PayloadHasValue()
        {
            var value = new ushort[] { 512, 2048 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt16(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<ushort>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromInt16_TimestampedValue_PayloadHasValue()
        {
            short value = 1024;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt16(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadInt16(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromInt16_TimestampedArray_PayloadHasValue()
        {
            var value = new short[] { 512, -2048 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt16(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<short>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt32_TimestampedValue_PayloadHasValue()
        {
            uint value = 123456789;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt32(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadUInt32(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt32_TimestampedArray_PayloadHasValue()
        {
            var value = new uint[] { uint.MaxValue / 2, 123456789 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt32(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<uint>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromInt32_TimestampedValue_PayloadHasValue()
        {
            int value = -123456789;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt32(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadInt32(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromInt32_TimestampedArray_PayloadHasValue()
        {
            var value = new int[] { int.MaxValue / 2, -123456789 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt32(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<int>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt64_TimestampedValue_PayloadHasValue()
        {
            ulong value = int.MaxValue * 2u;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt64(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadUInt64(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromUInt64_TimestampedArray_PayloadHasValue()
        {
            var value = new ulong[] { int.MaxValue * 2u, 123456789 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromUInt64(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<ulong>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromInt64_TimestampedValue_PayloadHasValue()
        {
            long value = -int.MaxValue * 2u;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt64(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadInt64(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromInt64_TimestampedArray_PayloadHasValue()
        {
            var value = new long[] { int.MaxValue * -2u, 123456789 };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromInt64(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<long>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }

        [TestMethod]
        public void FromSingle_TimestampedValue_PayloadHasValue()
        {
            float value = (float)Math.E;
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromSingle(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayloadSingle(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            Assert.AreEqual(value, payload);
        }

        [TestMethod]
        public void FromSingle_TimestampedArray_PayloadHasValue()
        {
            var value = new float[] { (float)Math.PI, 1 / 3f };
            var timestamp = GetTimestamp();
            var message = HarpMessage.FromSingle(DefaultAddress, timestamp, MessageType.Write, value);
            AssertIsValid(message);
            var payload = message.GetPayload<float>(out double actualTimestamp);
            AssertTimestamp(timestamp, actualTimestamp);
            AssertArrayEqual(value, payload);
        }
    }
}
