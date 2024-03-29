﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which creates an observable source of Harp messages.
    /// </summary>
    [Obsolete]
    [Description("Creates a new Harp message.")]
    public class CreateHarpMessage : Source<HarpMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateHarpMessage"/> class.
        /// </summary>
        public CreateHarpMessage()
        {
            MessageType = MessageType.Write;
            Address = 32;
            PayloadType = PayloadType.U8;
            Payload = 0;
        }

        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        [Description("The type of the message.")]
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the address of the register.
        /// </summary>
        [Description("The address of the register.")]
        public byte Address { get; set; }

        /// <summary>
        /// Gets or sets the type of the payload data.
        /// </summary>
        [Description("The type of the payload data.")]
        public PayloadType PayloadType { get; set; }

        /// <summary>
        /// Gets or sets the data to write on the message payload.
        /// </summary>
        [Description("The data to write on the message payload.")]
        public double Payload { get; set; }

        static HarpMessage CreateMessage(byte[] value, MessageType messageType, PayloadType payloadType, byte address, byte port)
        {
            byte checksum;
            byte[] frame;
            if (messageType == MessageType.Read)
            {
                checksum = (byte)((byte)messageType + 4 + address + port + (byte)payloadType);
                frame = new byte[] { (byte)messageType, 4, address, port, (byte)payloadType, checksum };
            }
            else
            {
                switch (payloadType)
                {
                    case PayloadType.U8:
                    case PayloadType.S8:
                        checksum = (byte)((byte)messageType + 5 + address + port + (byte)payloadType + value[0]);
                        frame = new byte[] { (byte)messageType, 5, address, port, (byte)payloadType, value[0], checksum };
                        break;
                    case PayloadType.U16:
                    case PayloadType.S16:
                        checksum = (byte)((byte)messageType + 6 + address + port + (byte)payloadType + value[0] + value[1]);
                        frame = new byte[] { (byte)messageType, 6, address, port, (byte)payloadType, value[0], value[1], checksum };
                        break;
                    case PayloadType.U32:
                    case PayloadType.S32:
                    case PayloadType.Float:
                        checksum = (byte)((byte)messageType + 8 + address + port + (byte)payloadType + value[0] + value[1] + value[2] + value[3]);
                        frame = new byte[] { (byte)messageType, 8, address, port, (byte)payloadType, value[0], value[1], value[2], value[3], checksum };
                        break;
                    case PayloadType.U64:
                    case PayloadType.S64:
                        checksum = (byte)((byte)messageType + 12 + address + port + (byte)payloadType + value[0] + value[1] + value[2] + value[3] + value[4] + value[5] + value[6] + value[7]);
                        frame = new byte[] { (byte)messageType, 12, address, port, (byte)payloadType, value[0], value[1], value[2], value[3], value[4], value[5], value[6], value[7], checksum };
                        break;
                    default:
                        throw new InvalidOperationException("No DataType defined.");
                }
            }
            
            return new HarpMessage(frame);
        }

        static byte[] PrepareSinglePayload(Single input, PayloadType dataType)
        {
            byte[] data;
            switch (dataType)
            {
                case PayloadType.U8:
                case PayloadType.U16:
                case PayloadType.U32:
                case PayloadType.U64:
                    UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                    data = BitConverter.GetBytes(dataUInt);
                    break;

                case PayloadType.S8:
                case PayloadType.S16:
                case PayloadType.S32:
                case PayloadType.S64:
                    Int64 dataInt = (Int64)(Convert.ToInt64(input));
                    data = BitConverter.GetBytes(dataInt);
                    break;

                case PayloadType.Float:
                    data = BitConverter.GetBytes(input);
                    break;

                default:
                    throw new InvalidOperationException("No DataType defined.");
            }

            return data;
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages.
        /// </summary>
        /// <returns>An observable sequence of Harp messages.</returns>
        public override IObservable<HarpMessage> Generate()
        {
            return Observable.Defer(() =>
            {
                byte[] data;
                switch (PayloadType)
                {
                    case PayloadType.U8:
                    case PayloadType.U16:
                    case PayloadType.U32:
                    case PayloadType.U64:
                        UInt64 dataUInt = (UInt64)Payload;
                        data = BitConverter.GetBytes(dataUInt);
                        break;
                    case PayloadType.S8:
                    case PayloadType.S16:
                    case PayloadType.S32:
                    case PayloadType.S64:
                        Int64 dataInt = (Int64)Payload;
                        data = BitConverter.GetBytes(dataInt);
                        break;
                    case PayloadType.Float:
                        var dataSingle = (Single)Payload;
                        data = BitConverter.GetBytes(dataSingle);
                        break;
                    default:
                        throw new InvalidOperationException("No DataType defined.");
                }
                
                var message = CreateMessage(data, MessageType, PayloadType, Address, 255);
                return Observable.Return(message);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<Byte> source)
        {
            return source.Select(input =>
            {
                UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                return CreateMessage(BitConverter.GetBytes(dataUInt), MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<SByte> source)
        {
            return source.Select(input =>
            {
                Int64 dataInt = (Int64)(Convert.ToInt64(input));
                return CreateMessage(BitConverter.GetBytes(dataInt), MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<UInt16> source)
        {
            return source.Select(input =>
            {
                UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                return CreateMessage(BitConverter.GetBytes(dataUInt), MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<Int16> source)
        {
            return source.Select(input =>
            {
                Int64 dataInt = (Int64)(Convert.ToInt64(input));
                return CreateMessage(BitConverter.GetBytes(dataInt), MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<UInt32> source)
        {
            return source.Select(input =>
            {
                UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                return CreateMessage(BitConverter.GetBytes(dataUInt), MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<Int32> source)
        {
            return source.Select(input =>
            {
                Int64 dataInt = (Int64)(Convert.ToInt64(input));
                return CreateMessage(BitConverter.GetBytes(dataInt), MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<UInt64> source)
        {
            return source.Select(input =>
            {
                UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                return CreateMessage(BitConverter.GetBytes(dataUInt), MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<Int64> source)
        {
            return source.Select(input =>
            {
                Int64 dataInt = (Int64)(Convert.ToInt64(input));
                return CreateMessage(BitConverter.GetBytes(dataInt), MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<Single> source)
        {
            return source.Select(input =>
            {
                var data = PrepareSinglePayload(input, PayloadType);
                return CreateMessage(data, MessageType, PayloadType, Address, 255);
            });
        }

        /// <summary>
        /// Returns an observable sequence of Harp messages from the input payload.
        /// </summary>
        /// <param name="source">An observable sequence representing the input payload.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public IObservable<HarpMessage> Generate(IObservable<Double> source)
        {
            return source.Select(input =>
            {
                var dataSingle = (Single)input;
                var data = PrepareSinglePayload(dataSingle, PayloadType);
                return CreateMessage(data, MessageType, PayloadType, Address, 255);
            });
        }
    }
}
