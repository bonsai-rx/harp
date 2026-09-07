using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Bonsai.Harp.Tests
{
    [TestClass]
    public class TestFormat
    {
        static Func<TSource, HarpMessage> CreateFormat<TSource>(FormatMessagePayload builder)
        {
            var source = Expression.Parameter(typeof(IObservable<TSource>));
            var body = builder.Build(new[] { source });
            var selector = Expression
                .Lambda<Func<IObservable<TSource>, IObservable<HarpMessage>>>(body, source)
                .Compile();
            return value => selector(Observable.Return(value)).Wait();
        }

        [TestMethod]
        public void TimestampedMessage_WithNoOverrides_CopiesMessageAndTakesSourceTimestamp()
        {
            var format = CreateFormat<Timestamped<HarpMessage>>(new FormatMessagePayload
            {
                Address = null,
                MessageType = null,
                PayloadType = null
            });

            var message = HarpMessage.FromByte(42, MessageType.Event, 7);
            var result = format(new Timestamped<HarpMessage>(message, 1.5));

            Assert.AreEqual(42, result.Address);
            Assert.AreEqual(MessageType.Event, result.MessageType);
            Assert.AreEqual(7, result.GetPayloadByte());
            Assert.IsTrue(result.IsTimestamped);
            Assert.AreEqual(1.5, result.GetTimestamp());
        }

        [TestMethod]
        public void TimestampedMessage_WithOverrides_UsesSpecifiedAddressAndType()
        {
            var format = CreateFormat<Timestamped<HarpMessage>>(new FormatMessagePayload
            {
                Address = 20,
                MessageType = MessageType.Write,
                PayloadType = null
            });

            var message = HarpMessage.FromByte(42, MessageType.Event, 7);
            var result = format(new Timestamped<HarpMessage>(message, 1.5));

            Assert.AreEqual(20, result.Address);
            Assert.AreEqual(MessageType.Write, result.MessageType);
            Assert.AreEqual(7, result.GetPayloadByte());
            Assert.AreEqual(1.5, result.GetTimestamp());
        }

        [TestMethod]
        public void TimestampedMessage_WithSpecifiedPayloadType_InfersMissingAddress()
        {
            var format = CreateFormat<Timestamped<HarpMessage>>(new FormatMessagePayload
            {
                Address = null,
                MessageType = null,
                PayloadType = PayloadType.TimestampedU8
            });

            var message = HarpMessage.FromByte(42, MessageType.Event, 7);
            var result = format(new Timestamped<HarpMessage>(message, 1.5));

            Assert.AreEqual(42, result.Address);
            Assert.AreEqual(MessageType.Event, result.MessageType);
            Assert.AreEqual(7, result.GetPayloadByte());
            Assert.AreEqual(1.5, result.GetTimestamp());
        }

        [TestMethod]
        public void TimestampedValue_WithSpecifiedPayloadType_FormatsTimestampedMessage()
        {
            var format = CreateFormat<Timestamped<byte>>(new FormatMessagePayload
            {
                Address = 20,
                MessageType = MessageType.Write,
                PayloadType = PayloadType.TimestampedU8
            });

            var result = format(new Timestamped<byte>(7, 1.5));

            Assert.AreEqual(20, result.Address);
            Assert.AreEqual(MessageType.Write, result.MessageType);
            Assert.AreEqual(7, result.GetPayloadByte());
            Assert.AreEqual(1.5, result.GetTimestamp());
        }

        [TestMethod]
        public void TimestampedValue_WithPlainPayloadType_DiscardsTimestamp()
        {
            var format = CreateFormat<Timestamped<byte>>(new FormatMessagePayload
            {
                Address = 20,
                MessageType = MessageType.Write,
                PayloadType = PayloadType.U8
            });

            var result = format(new Timestamped<byte>(7, 1.5));

            Assert.AreEqual(20, result.Address);
            Assert.AreEqual(7, result.GetPayloadByte());
            Assert.IsFalse(result.IsTimestamped);
        }

        [TestMethod]
        public void TimestampedMessage_WithPlainPayloadType_DiscardsTimestamp()
        {
            var format = CreateFormat<Timestamped<HarpMessage>>(new FormatMessagePayload
            {
                Address = null,
                MessageType = null,
                PayloadType = PayloadType.U8
            });

            var message = HarpMessage.FromByte(42, MessageType.Event, 7);
            var result = format(new Timestamped<HarpMessage>(message, 1.5));

            Assert.AreEqual(42, result.Address);
            Assert.AreEqual(MessageType.Event, result.MessageType);
            Assert.AreEqual(7, result.GetPayloadByte());
            Assert.IsFalse(result.IsTimestamped);
        }

        [TestMethod]
        public void TimestampedMessage_WrappingTimestampedMessage_WithPlainPayloadType_DiscardsBoth()
        {
            var format = CreateFormat<Timestamped<HarpMessage>>(new FormatMessagePayload
            {
                Address = null,
                MessageType = null,
                PayloadType = PayloadType.U8
            });

            var message = HarpMessage.FromByte(42, 9.5, MessageType.Event, 7);
            var result = format(new Timestamped<HarpMessage>(message, 1.5));

            Assert.AreEqual(42, result.Address);
            Assert.AreEqual(7, result.GetPayloadByte());
            Assert.IsFalse(result.IsTimestamped);
        }

        [TestMethod]
        public void TimestampedMessage_WrappingTimestampedMessage_TakesWrapperTimestamp()
        {
            var format = CreateFormat<Timestamped<HarpMessage>>(new FormatMessagePayload
            {
                Address = null,
                MessageType = null,
                PayloadType = null
            });

            var message = HarpMessage.FromByte(42, 9.5, MessageType.Event, 7);
            var result = format(new Timestamped<HarpMessage>(message, 1.5));

            Assert.AreEqual(42, result.Address);
            Assert.AreEqual(MessageType.Event, result.MessageType);
            Assert.AreEqual(PayloadType.TimestampedU8, result.PayloadType);
            Assert.AreEqual(7, result.GetPayloadByte());
            Assert.AreEqual(1.5, result.GetTimestamp());
        }

        [TestMethod]
        public void Message_WithNoOverrides_PreservesOwnTimestamp()
        {
            var format = CreateFormat<HarpMessage>(new FormatMessagePayload
            {
                Address = null,
                MessageType = null,
                PayloadType = null
            });

            var timestamped = format(HarpMessage.FromByte(42, 1.5, MessageType.Event, 7));
            Assert.AreEqual(42, timestamped.Address);
            Assert.AreEqual(MessageType.Event, timestamped.MessageType);
            Assert.AreEqual(1.5, timestamped.GetTimestamp());

            var plain = format(HarpMessage.FromByte(42, MessageType.Event, 7));
            Assert.AreEqual(42, plain.Address);
            Assert.IsFalse(plain.IsTimestamped);
        }
    }
}
