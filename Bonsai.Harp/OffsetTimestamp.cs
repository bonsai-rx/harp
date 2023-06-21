using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that shifts the timestamps of a sequence of timestamped payload
    /// values by the specified time interval.
    /// </summary>
    [Combinator]
    [Description("Shifts the timestamps of a sequence of timestamped payload values by the specified time interval.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class OffsetTimestamp
    {
        /// <summary>
        /// Gets or sets a time interval by which to offset the sequence timestamps.
        /// </summary>
        [XmlIgnore]
        [Description("The time interval by which to offset the sequence timestamps.")]
        public TimeSpan TimeShift { get; set; }

        /// <summary>
        /// Gets or sets an XML representation of the offset time interval for serialization.
        /// </summary>
        [Browsable(false)]
        [XmlElement(nameof(TimeShift))]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TimeShiftXml
        {
            get => XmlConvert.ToString(TimeShift);
            set => TimeShift = XmlConvert.ToTimeSpan(value);
        }

        /// <summary>
        /// Shifts the timestamps of an observable sequence of timestamped payload values
        /// by the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the values in each timestamped payload.</typeparam>
        /// <param name="source">An observable sequence of timestamped payload values.</param>
        /// <returns>
        /// A sequence of timestamped payload values where each timestamp is computed by adding
        /// the time interval offset to the timestamp of the corresponding payload in the
        /// <paramref name="source"/> sequence.
        /// </returns>
        public IObservable<Timestamped<T>> Process<T>(IObservable<Timestamped<T>> source)
        {
            return source.Select(x => Timestamped.Create(x.Value, x.Seconds + TimeShift.TotalSeconds));
        }

        /// <summary>
        /// Shifts the timestamps of an observable sequence of timestamped payload values
        /// by the specified offset, in fractional seconds.
        /// </summary>
        /// <typeparam name="T">The type of the values in each timestamped payload.</typeparam>
        /// <param name="source">
        /// A sequence of pairs containing a timestamped payload and a time interval, in
        /// fractional seconds, by which to offset the payload timestamp.
        /// </param>
        /// <returns>
        /// A sequence of timestamped payload values where each timestamp is computed by adding
        /// both the seconds offset and the base <see cref="TimeShift"/> offset to the timestamp
        /// of the corresponding payload in the <paramref name="source"/> sequence.
        /// </returns>
        public IObservable<Timestamped<T>> Process<T>(IObservable<Tuple<Timestamped<T>, double>> source)
        {
            return source.Select(x => Timestamped.Create(
                x.Item1.Value,
                x.Item1.Seconds + x.Item2 + TimeShift.TotalSeconds));
        }

        /// <summary>
        /// Shifts the timestamps of an observable sequence of timestamped payload values
        /// by the specified time interval.
        /// </summary>
        /// <typeparam name="T">The type of the values in each timestamped payload.</typeparam>
        /// <param name="source">
        /// A sequence of pairs containing a timestamped payload and the time interval by
        /// which to offset the payload timestamp.
        /// </param>
        /// <returns>
        /// A sequence of timestamped payload values where each timestamp is computed by adding
        /// both the time interval and the base <see cref="TimeShift"/> offset to the timestamp
        /// of the corresponding payload in the <paramref name="source"/> sequence.
        /// </returns>
        public IObservable<Timestamped<T>> Process<T>(IObservable<Tuple<Timestamped<T>, TimeSpan>> source)
        {
            return source.Select(x => Timestamped.Create(
                x.Item1.Value,
                x.Item1.Seconds + x.Item2.TotalSeconds + TimeShift.TotalSeconds));
        }
    }
}
