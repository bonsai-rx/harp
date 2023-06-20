using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that creates a sequence of timestamped payload values from a
    /// sequence of value-timestamp pairs.
    /// </summary>
    [Combinator]
    [Description("Creates a sequence of timestamped payload values from a sequence of value-timestamp pairs.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public sealed class CreateTimestamped
    {
        internal static readonly DateTime ReferenceTime = new DateTime(1904, 1, 1);

        /// <summary>
        /// Creates an observable sequence of timestamped payload values from a sequence
        /// of value-timestamp pairs.
        /// </summary>
        /// <typeparam name="T">The type of the value in the timestamped payload.</typeparam>
        /// <param name="source">
        /// A sequence of value-timestamp pairs, where the second element specifies the
        /// timestamp of the payload, in fractional seconds.
        /// </param>
        /// <returns>
        /// An observable sequence of timestamped payload values.
        /// </returns>
        public IObservable<Timestamped<T>> Process<T>(IObservable<Tuple<T, double>> source)
        {
            return source.Select(value => Timestamped.Create(value.Item1, value.Item2));
        }

        /// <summary>
        /// Creates an observable sequence of timestamped payload values from a sequence
        /// of value-message pairs.
        /// </summary>
        /// <typeparam name="T">The type of the value in the timestamped payload.</typeparam>
        /// <param name="source">
        /// A sequence of value-message pairs, where the second element is a <see cref="HarpMessage"/>
        /// specifying the timestamp of the payload, in fractional seconds.
        /// </param>
        /// <returns>
        /// An observable sequence of timestamped payload values.
        /// </returns>
        public IObservable<Timestamped<T>> Process<T>(IObservable<Tuple<T, HarpMessage>> source)
        {
            return source.Select(value => Timestamped.Create(value.Item1, value.Item2.GetTimestamp()));
        }

        /// <summary>
        /// Creates an observable sequence of timestamped message values surfacing the
        /// timestamp of the message object in fractional seconds.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="HarpMessage"/> objects.
        /// </param>
        /// <returns>
        /// An observable sequence of timestamped values representing the original
        /// message and its timestamp, in fractional seconds.
        /// </returns>
        public IObservable<Timestamped<HarpMessage>> Process(IObservable<HarpMessage> source)
        {
            return source.Select(message => Timestamped.Create(message, message.GetTimestamp()));
        }

        /// <summary>
        /// Creates an observable sequence of timestamped message values by converting
        /// the timestamp in the local scheduler clock to a Harp timestamp, in fractional
        /// seconds relative to the reference time.
        /// </summary>
        /// <typeparam name="T">The type of the value in the timestamped payload.</typeparam>
        /// <param name="source">
        /// A sequence of <see cref="System.Reactive.Timestamped{T}"/> values specifying the
        /// timestamp in a local scheduler clock.
        /// </param>
        /// <returns>
        /// An observable sequence of Harp timestamped payload values.
        /// </returns>
        public IObservable<Timestamped<T>> Process<T>(IObservable<System.Reactive.Timestamped<T>> source)
        {
            return source.Select(_ =>
            {
                var seconds = _.Timestamp.UtcDateTime.Subtract(ReferenceTime).TotalSeconds;
                return Timestamped.Create(_.Value, seconds);
            });
        }
    }
}
