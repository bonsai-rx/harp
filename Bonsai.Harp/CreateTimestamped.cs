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
    }
}
