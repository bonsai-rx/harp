using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.Harp
{
    [Description("Demultiplexes the bit digital state into independent channels.")]
    public class BitToMat : Source<Mat>
    {
        [Description("Number of bits to demultiplex")]
        public byte NumberOfBits { get; set; }

        public override IObservable<Mat> Generate()
        {
            return Observable.Defer(() =>
            {
                /* Empty 8 positionx matrix */
                Mat mat = new Mat(8, 1, Depth.U8, 1);
                return Observable.Return(mat);
            });
        }

        public IObservable<Mat> Generate(IObservable<Byte> source)
        {
            return source.Select(input =>
            {
                if (NumberOfBits > 8)
                    throw new InvalidOperationException("Number of bits to demultiplex not compatible with the input type.");

                var output = new Mat(NumberOfBits, 1, Depth.U8, 1);
                for (int i = 0; i < output.Rows; i++)
                {
                    using (var row = output.GetRow(i))
                    {
                        row.SetReal(0, (input >> i) & 1);
                        // CV.AndS(input, Scalar.Real(1 << i), row);
                    }
                }
                return output;
            });
        }

        public IObservable<Mat> Generate(IObservable<UInt16> source)
        {
            return source.Select(input =>
            {
                if (NumberOfBits > 16)
                    throw new InvalidOperationException("Number of bits to demultiplex not compatible with the input type.");

                var output = new Mat(NumberOfBits, 1, Depth.U8, 1);
                for (int i = 0; i < output.Rows; i++)
                {
                    using (var row = output.GetRow(i))
                    {
                        row.SetReal(0, (input >> i) & 1);
                        // CV.AndS(input, Scalar.Real(1 << i), row);
                    }
                }
                return output;
            });
        }

        public IObservable<Mat> Generate(IObservable<UInt32> source)
        {
            return source.Select(input =>
            {
                if (NumberOfBits > 32)
                    throw new InvalidOperationException("Number of bits to demultiplex not compatible with the input type.");

                var output = new Mat(NumberOfBits, 1, Depth.U8, 1);
                for (int i = 0; i < output.Rows; i++)
                {
                    using (var row = output.GetRow(i))
                    {
                        row.SetReal(0, (input >> i) & 1);
                        // CV.AndS(input, Scalar.Real(1 << i), row);
                    }
                }
                return output;
            });
        }
    }
}

