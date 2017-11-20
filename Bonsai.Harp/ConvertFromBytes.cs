using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Bonsai.Harp
{
    [Description("Converts a byte array into an HarpDataFrame. The byte array must contain a complete and incorrupt packet")]
    public class ConvertFromBytes : Transform<byte[], HarpDataFrame>
    {
        public override IObservable<HarpDataFrame> Process(IObservable<byte[]> source)
        {
            return source.Select(input =>
            {
                return new HarpDataFrame(input);
            });
        }
    }
}