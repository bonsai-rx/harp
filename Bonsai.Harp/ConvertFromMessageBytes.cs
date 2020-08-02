using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents a transform operator which converts an observable sequence of byte arrays into a sequence of Harp messages.
    /// </summary>
    [Description("Converts a byte array into a Harp message. The byte array should represent the complete message bytes.")]
    public class ConvertFromMessageBytes : Transform<byte[], HarpMessage>
    {
        /// <summary>
        /// Converts an observable sequence of byte arrays into a sequence of Harp messages.
        /// Each array of bytes should contain the full binary representation of a Harp message.
        /// </summary>
        /// <param name="source">An observable sequence of byte arrays.</param>
        /// <returns>An observable sequence of Harp messages.</returns>
        public override IObservable<HarpMessage> Process(IObservable<byte[]> source)
        {
            return source.Select(input => new HarpMessage(input));
        }
    }
}