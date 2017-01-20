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
    //public class CreateTimestamp<T>
    //{
    //    public IObservable<Timestamped<T>> Process(IObservable<Tuple<T, double>> source)
    //    {
    //        return source.Select(input => new Timestamped<T>(input.Item1, input.Item2));
    //    }

    //}

    
    
    public class CreateHarpDataFrame : Source<HarpDataFrame>
    {
        HarpDataFrame StaticFrame;

        [Description("Operation Type: Write (Bonsai to device) or Read (device to Bonsai).")]
        public MessageId Operation { get; set; }

        [Description("The address of the register.")]
        public byte AddressRegister { get; set; }

        [Description("Type of data.")]
        public HarpType DataType { get; set; }

        [Description("The value to write.")]
        //public Double Data { get; set; }
        public string Data { get; set; }

        static HarpDataFrame CreateFrame(byte[] value, MessageId msgId, HarpType type, byte reagAdd, byte port)
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
                    case HarpType.U8:
                    case HarpType.I8:
                        checksum = (byte)((byte)msgId + 5 + reagAdd + port + (byte)type + value[0]);
                        frame = new byte[] { (byte)msgId, 5, reagAdd, port, (byte)type, value[0], checksum };
                        break;
                    case HarpType.U16:
                    case HarpType.I16:
                        checksum = (byte)((byte)msgId + 6 + reagAdd + port + (byte)type + value[0] + value[1]);
                        frame = new byte[] { (byte)msgId, 6, reagAdd, port, (byte)type, value[0], value[1], checksum };
                        break;
                    case HarpType.U32:
                    case HarpType.I32:
                    case HarpType.Float:
                        checksum = (byte)((byte)msgId + 8 + reagAdd + port + (byte)type + value[0] + value[1] + value[2] + value[3]);
                        frame = new byte[] { (byte)msgId, 8, reagAdd, port, (byte)type, value[0], value[1], value[2], value[3], checksum };
                        break;
                    case HarpType.U64:
                    case HarpType.I64:
                        checksum = (byte)((byte)msgId + 12 + reagAdd + port + (byte)type + value[0] + value[1] + value[2] + value[3] + value[4] + value[5] + value[6] + value[7]);
                        frame = new byte[] { (byte)msgId, 12, reagAdd, port, (byte)type, value[0], value[1], value[2], value[3], value[4], value[5], value[6], value[7], checksum };
                        break;
                    default:
                        throw new InvalidOperationException("No DataType defined.");
                }
            }
            
            return new HarpDataFrame(frame);
        }

        static byte[] PrepareSinglePayload(Single input, HarpType dataType)
        {
            byte[] data;

            switch (dataType)
            {
                case HarpType.U8:
                case HarpType.U16:
                case HarpType.U32:
                case HarpType.U64:
                    UInt64 dataUInt = (UInt64)(Convert.ToUInt64(input));
                    data = BitConverter.GetBytes(dataUInt);
                    break;

                case HarpType.I8:
                case HarpType.I16:
                case HarpType.I32:
                case HarpType.I64:
                    Int64 dataInt = (Int64)(Convert.ToInt64(input));
                    data = BitConverter.GetBytes(dataInt);
                    break;

                case HarpType.Float:
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
            DataType = HarpType.U8;
            //Data = 0;
            Data = "0";
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
                        case HarpType.U8:
                        case HarpType.U16:
                        case HarpType.U32:
                        case HarpType.U64:
                            //UInt64 dataUInt = (UInt64)Data;
                            UInt64 dataUInt = (UInt64)(Convert.ToUInt64(Data, CultureInfo.InvariantCulture));
                            data = BitConverter.GetBytes(dataUInt);
                            break;

                        case HarpType.I8:
                        case HarpType.I16:
                        case HarpType.I32:
                        case HarpType.I64:
                            //Int64 dataInt = (Int64)Data;
                            Int64 dataInt = (Int64)(Convert.ToInt64(Data, CultureInfo.InvariantCulture));
                            data = BitConverter.GetBytes(dataInt);
                            break;

                        case HarpType.Float:
                            //var dataSingle = (Single)Data;
                            Single dataSingle = (Single)(Convert.ToSingle(Data, CultureInfo.InvariantCulture));
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
