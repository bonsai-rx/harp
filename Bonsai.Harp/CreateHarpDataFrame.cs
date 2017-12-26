using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    public class CreateHarpDataFrame : Source<HarpDataFrame>
    {
        HarpDataFrame StaticFrame;

        [Description("Operation Type: Write (Bonsai to device) or Read (device to Bonsai).")]
        public MessageId Operation { get; set; }

        [Description("The address of the register.")]
        public byte AddressRegister { get; set; }

        [Description("Type of data.")]
        public PayloadType DataType { get; set; }

        [Description("The value to write.")]
        public Double Data { get; set; }

        static HarpDataFrame CreateFrame(byte[] value, MessageId msgId, PayloadType type, byte reagAdd, byte port)
        {
            
            byte checksum;
            byte[] frame;

            if (msgId == MessageId.Read)
            {
                checksum = (byte)((byte)msgId + 4 + reagAdd + port + (byte)type);
                frame = new byte[] { (byte)msgId, 4, reagAdd, port, (byte)type, checksum };
            }
            else
            {
                switch (type)
                {
                    case PayloadType.U8:
                    case PayloadType.S8:
                        checksum = (byte)((byte)msgId + 5 + reagAdd + port + (byte)type + value[0]);
                        frame = new byte[] { (byte)msgId, 5, reagAdd, port, (byte)type, value[0], checksum };
                        break;
                    case PayloadType.U16:
                    case PayloadType.S16:
                        checksum = (byte)((byte)msgId + 6 + reagAdd + port + (byte)type + value[0] + value[1]);
                        frame = new byte[] { (byte)msgId, 6, reagAdd, port, (byte)type, value[0], value[1], checksum };
                        break;
                    case PayloadType.U32:
                    case PayloadType.S32:
                    case PayloadType.Float:
                        checksum = (byte)((byte)msgId + 8 + reagAdd + port + (byte)type + value[0] + value[1] + value[2] + value[3]);
                        frame = new byte[] { (byte)msgId, 8, reagAdd, port, (byte)type, value[0], value[1], value[2], value[3], checksum };
                        break;
                    case PayloadType.U64:
                    case PayloadType.S64:
                        checksum = (byte)((byte)msgId + 12 + reagAdd + port + (byte)type + value[0] + value[1] + value[2] + value[3] + value[4] + value[5] + value[6] + value[7]);
                        frame = new byte[] { (byte)msgId, 12, reagAdd, port, (byte)type, value[0], value[1], value[2], value[3], value[4], value[5], value[6], value[7], checksum };
                        break;
                    default:
                        throw new InvalidOperationException("No DataType defined.");
                }
            }
            
            return new HarpDataFrame(frame);
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


        public CreateHarpDataFrame()
        {
            /* Runs when the user put the node on the Bonsai Workflow */
            Operation = MessageId.Write;
            AddressRegister = 32;
            DataType = PayloadType.U8;
            Data = 0;
        }

        public override IObservable<HarpDataFrame> Generate()
        {
            return Observable.Defer(() =>
            {
                byte[] data;

                try
                {
                    switch (DataType)
                    {
                        case PayloadType.U8:
                        case PayloadType.U16:
                        case PayloadType.U32:
                        case PayloadType.U64:
                            UInt64 dataUInt = (UInt64)Data;
                            data = BitConverter.GetBytes(dataUInt);
                            break;

                        case PayloadType.S8:
                        case PayloadType.S16:
                        case PayloadType.S32:
                        case PayloadType.S64:
                            Int64 dataInt = (Int64)Data;
                            data = BitConverter.GetBytes(dataInt);
                            break;

                        case PayloadType.Float:
                            var dataSingle = (Single)Data;
                            data = BitConverter.GetBytes(dataSingle);
                            break;

                        default:
                            throw new InvalidOperationException("No DataType defined.");
                    }
                }
                catch (Exception)
                {
                    throw new InvalidOperationException("The Data field is not correct. It doesn't fit the selected DataType");
                }
                
                StaticFrame = CreateFrame(data, Operation, DataType, AddressRegister, 255);
                return Observable.Return(StaticFrame);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<Byte> source)
        {
            return source.Select(input =>
            {
                UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                return CreateFrame(BitConverter.GetBytes(dataUInt), Operation, DataType, AddressRegister, 255);
            });
        }
        
        public IObservable<HarpDataFrame> Generate(IObservable<SByte> source)
        {
            return source.Select(input =>
            {
                Int64 dataInt = (Int64)(Convert.ToInt64(input));
                return CreateFrame(BitConverter.GetBytes(dataInt), Operation, DataType, AddressRegister, 255);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<UInt16> source)
        {
            return source.Select(input =>
            {
                UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                return CreateFrame(BitConverter.GetBytes(dataUInt), Operation, DataType, AddressRegister, 255);
            });
        }
        
        public IObservable<HarpDataFrame> Generate(IObservable<Int16> source)
        {
            return source.Select(input =>
            {
                Int64 dataInt = (Int64)(Convert.ToInt64(input));
                return CreateFrame(BitConverter.GetBytes(dataInt), Operation, DataType, AddressRegister, 255);
            });
        }
        
        public IObservable<HarpDataFrame> Generate(IObservable<UInt32> source)
        {
            return source.Select(input =>
            {
                UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                return CreateFrame(BitConverter.GetBytes(dataUInt), Operation, DataType, AddressRegister, 255);
            });
        }
        
        public IObservable<HarpDataFrame> Generate(IObservable<Int32> source)
        {
            return source.Select(input =>
            {
                Int64 dataInt = (Int64)(Convert.ToInt64(input));
                return CreateFrame(BitConverter.GetBytes(dataInt), Operation, DataType, AddressRegister, 255);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<UInt64> source)
        {
            return source.Select(input =>
            {
                UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                return CreateFrame(BitConverter.GetBytes(dataUInt), Operation, DataType, AddressRegister, 255);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<Int64> source)
        {
            return source.Select(input =>
            {
                Int64 dataInt = (Int64)(Convert.ToInt64(input));
                return CreateFrame(BitConverter.GetBytes(dataInt), Operation, DataType, AddressRegister, 255);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<Single> source)
        {
            return source.Select(input =>
            {
                var data = PrepareSinglePayload(input, DataType);
                return CreateFrame(data, Operation, DataType, AddressRegister, 255);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<Double> source)
        {
            return source.Select(input =>
            {
                var dataSingle = (Single)input;
                var data = PrepareSinglePayload(dataSingle, DataType);
                return CreateFrame(data, Operation, DataType, AddressRegister, 255);
            });
        }



    }
}
