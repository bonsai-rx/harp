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
    [Description("Demultiplexes the TTL digital state into independent channels.")]
    public class TtlState : Transform<UInt16, Mat>
    {
        public override IObservable<Mat> Process(IObservable<UInt16> source)
        {
            return source.Select(input =>
            {
                var output = new Mat(16, 1, Depth.U8, 1);
                for (int i = 0; i < output.Rows; i++)
                {
                    using (var row = output.GetRow(i))
                    {
                        row.SetReal(0, (input >> i) & 1);
                    }
                }
                return output;
            });
        }

        public IObservable<Mat> Process(IObservable<byte> source)
        {
            return source.Select(input =>
            {
                var output = new Mat(8, 1, Depth.U8, 1);
                for (int i = 0; i < output.Rows; i++)
                {
                    using (var row = output.GetRow(i))
                    {
                        row.SetReal(0, (input >> i) & 1);
                    }
                }
                return output;
            });
        }

        public IObservable<Mat> Process(IObservable<uint> source)
        {
            return source.Select(input =>
            {
                var output = new Mat(32, 1, Depth.U8, 1);
                for (int i = 0; i < output.Rows; i++)
                {
                    using (var row = output.GetRow(i))
                    {
                        row.SetReal(0, (input >> i) & 1);
                    }
                }
                return output;
            });
        }
    }
}