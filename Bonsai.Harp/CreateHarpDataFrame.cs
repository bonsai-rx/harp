using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public byte[] Message { get; set; }

        [Description("The Address for which to write values.")]
        public byte AddressRegister { get; set; }

        [Description("The Port for which to write values.")]
        public byte AddressDevice { get; set; }

        //public double? Timestamp { get; set; }

        public CreateHarpDataFrame()
        {
            //if(Timestamp.HasValue) // create timestamped message
            //else // create raw message
            Message = new byte[] { 2, 5, 10, 255, 1, 97, 114 };
        }

        public override IObservable<HarpDataFrame> Generate()
        {
            return Observable.Defer(() =>
            {
                var dataFrame = new HarpDataFrame(Message);
                return Observable.Return(dataFrame);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<Byte> source)
        {
            return source.Select(input =>
            {
                byte sum = (byte)(2 + 5 + AddressRegister + AddressDevice + 1 + input);
                Message = new byte[] { 2, 5, AddressRegister, AddressDevice, 1, input, sum };
                return new HarpDataFrame(Message);
            });
        }

        // not tested
        public IObservable<HarpDataFrame> Generate(IObservable<SByte> source)
        {
            return source.Select(input =>
            {
                byte sum = (byte)(2 + 5 + AddressRegister + AddressDevice + 0x81 + input);
                Message = new byte[] { 2, 5, AddressRegister, AddressDevice, 0x81, (byte)input, sum };
                return new HarpDataFrame(Message);
            });
        }

        public IObservable<HarpDataFrame> Generate(IObservable<UInt16> source)
        {
            return source.Select(input =>
            {
                byte low = (byte) (input & 0x00FF);
                byte high = (byte )((input>>8) & 0x00FF);
                byte[] inputbytes = BitConverter.GetBytes(input);
                byte sum = (byte)(2 + 6 + AddressRegister + AddressDevice + 2 + low + high);
                Message = new byte[] { 2, 6, AddressRegister, AddressDevice, 2, low, high, sum };
                return new HarpDataFrame(Message);
            });
        }

        // not tested
        public IObservable<HarpDataFrame> Generate(IObservable<Int16> source)
        {
            return source.Select(input =>
            {
                byte[] inputBytes = BitConverter.GetBytes(input);
                byte sum = (byte)(2 + 6 + AddressRegister + AddressDevice + 0x82 + inputBytes[0] + inputBytes[1]);
                Message = new byte[] { 2, 6, AddressRegister, AddressDevice, 0x82, inputBytes[0], inputBytes[1], sum };
                return new HarpDataFrame(Message);
            });
        }

        // not tested
        public IObservable<HarpDataFrame> Generate(IObservable<UInt32> source)
        {
            return source.Select(input =>
            {
                byte[] inputBytes = BitConverter.GetBytes(input);
                byte sum = (byte)(2 + 8 + AddressRegister + AddressDevice + 4 + inputBytes[0] + inputBytes[1] + inputBytes[2] + inputBytes[3]);
                Message = new byte[] { 2, 8, AddressRegister, AddressDevice, 4, inputBytes[0], inputBytes[1], inputBytes[2], inputBytes[3], sum };
                return new HarpDataFrame(Message);
            });
        }

        // not tested
        public IObservable<HarpDataFrame> Generate(IObservable<Int32> source)
        {
            return source.Select(input =>
            {
                byte[] inputBytes = BitConverter.GetBytes(input);
                byte sum = (byte)(2 + 8 + AddressRegister + AddressDevice + 0x84 + inputBytes[0] + inputBytes[1] + inputBytes[2] + inputBytes[3]);
                Message = new byte[] { 2, 8, AddressRegister, AddressDevice, 0x84, inputBytes[0], inputBytes[1], inputBytes[2], inputBytes[3], sum };
                return new HarpDataFrame(Message);
            });
        }

        // not tested check
        public IObservable<HarpDataFrame> Generate(IObservable<Single> source)
        {
            return source.Select(input =>
            {
                byte[] inputBytes = BitConverter.GetBytes(input);
                byte sum = (byte)(2 + 8 + AddressRegister + AddressDevice + 0x44 + inputBytes[0] + inputBytes[1] + inputBytes[2] + inputBytes[3]);
                Message = new byte[] { 2, 8, AddressRegister, AddressDevice, 0x44, inputBytes[0], inputBytes[1], inputBytes[2], inputBytes[3], sum };
                return new HarpDataFrame(Message);
            });
        }




    }
}
