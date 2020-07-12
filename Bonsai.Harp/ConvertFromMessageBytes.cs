using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp
{
    [Description("Converts a byte array into a Harp message. The byte array should represent the complete message bytes.")]
    public class ConvertFromMessageBytes : Transform<byte[], HarpMessage>
    {
        public override IObservable<HarpMessage> Process(IObservable<byte[]> source)
        {
            return source.Select(input =>
            {
                return new HarpMessage(input);
            });
        }
    }
}