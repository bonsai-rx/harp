using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that creates a command message to set the value of
    /// the timestamp register in the Harp device to the current UTC time of the host.
    /// </summary>
    [Description("Creates a command message to set the value of the timestamp register in the Harp device to the current UTC time of the host.")]
    public class SynchronizeTimestamp : Combinator<HarpMessage>
    {
        /// <summary>
        /// Creates an observable sequence of command messages to set the value of
        /// the timestamp register in the Harp device to the current UTC time of
        /// the host.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for creating new command
        /// messages.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing the command
        /// to set the value of the timestamp register in the Harp device.
        /// </returns>
        public override IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ =>
            {
                var timestamp = (uint)DateTime.UtcNow.Subtract(CreateTimestamped.ReferenceTime).TotalSeconds;
                return TimestampSeconds.FromPayload(MessageType.Write, timestamp);
            });
        }
    }
}
