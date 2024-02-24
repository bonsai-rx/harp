using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bonsai.Expressions;

namespace Bonsai.Harp.Visualizers
{
    /// <summary>
    /// Represents an operator that configures a visualizer to plot each Harp message
    /// in the sequence in a synchronized rolling graph.
    /// </summary>
    [TypeVisualizer(typeof(TriggerTimelineGraphVisualizer))]
    [Description("A visualizer that plots each Harp message in the sequence in a synchronized rolling graph.")]
    public class TriggerTimelineGraphBuilder : ExpressionBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 2, upperBound: 2);

        /// <summary>
        /// Gets the range of input arguments that this expression builder accepts.
        /// </summary>
        public override Range<int> ArgumentRange
        {
            get { return argumentRange; }
        }

        /// <summary>
        /// Gets or sets the optional maximum time range captured in the timeline graph.
        /// If no time span is specified, all data points will be displayed.
        /// </summary>
        [Category("Range")]
        [Description("The optional maximum time range captured in the timeline graph. If no time span is specified, all data points will be displayed.")]
        public double? TimeSpan { get; set; }

        internal VisualizerController Controller { get; set; }

        internal class VisualizerController
        {
            internal double? TimeSpan;
            internal ReplaySubject<IGroupedObservable<Timestamped<double>, Timestamped<LabeledRegister>>> Triggers;
        }

        internal struct LabeledRegister
        {
            public string Label;
            public int Address;

            public LabeledRegister(string label, int address)
            {
                Label = label;
                Address = address;
            }
        }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var sources = arguments.ToArray();
            var triggerType = sources[1].Type.GetGenericArguments()[0];
            if (!triggerType.IsGenericType || triggerType.GetGenericTypeDefinition() != typeof(Timestamped<>))
            {
                throw new InvalidOperationException("The trigger input must be Harp timestamped.");
            }

            triggerType = triggerType.GetGenericArguments()[0];
            Controller = new VisualizerController
            {
                TimeSpan = TimeSpan,
                Triggers = new()
            };
            var combinator = Expression.Constant(this);
            return Expression.Call(combinator, nameof(Process), new[] { triggerType }, sources);
        }

        IObservable<TSource> Process<TSource, TTrigger>(
            IObservable<TSource> source,
            Func<IObservable<TSource>, IObservable<Timestamped<LabeledRegister>>> selector,
            IObservable<Timestamped<TTrigger>> trigger)
        {
            return source.Publish(ps => trigger.Publish(pt => ps.Merge(
                selector(ps).Window(pt).Skip(1).Zip(pt, (window, offset) =>
                    TimelineObservable.Create(
                        Timestamped.Create(Convert.ToDouble(offset.Value), offset.Seconds),
                        window))
                .Do(Controller.Triggers)
                .IgnoreElements()
                .Cast<TSource>())));
        }

        IObservable<HarpMessage> Process<TTrigger>(
            IObservable<HarpMessage> source,
            IObservable<Timestamped<TTrigger>> trigger)
        {
            return Process(source, ps => ps.Select(
                message => Timestamped.Create(
                    new LabeledRegister(message.Address.ToString(), message.Address),
                    message.GetTimestamp())),
                trigger);
        }

        IObservable<IGroupedObservable<int, HarpMessage>> Process<TTrigger>(
            IObservable<IGroupedObservable<int, HarpMessage>> source,
            IObservable<Timestamped<TTrigger>> trigger)
        {
            return Process(source, ps => ps.SelectMany(group => group.Select(
                message => Timestamped.Create(
                    new LabeledRegister(message.Address.ToString(), message.Address),
                    message.GetTimestamp()))),
                trigger);
        }

        IObservable<IGroupedObservable<Type, HarpMessage>> Process<TTrigger>(
            IObservable<IGroupedObservable<Type, HarpMessage>> source,
            IObservable<Timestamped<TTrigger>> trigger)
        {
            return Process(source, ps => ps.SelectMany(group => group.Select(
                message => Timestamped.Create(
                    new LabeledRegister(group.Key.Name, message.Address),
                    message.GetTimestamp()))),
                trigger);
        }

        static class TimelineObservable
        {
            public static IGroupedObservable<Timestamped<double>, Timestamped<TLabel>> Create<TLabel>(
                Timestamped<double> key, IObservable<Timestamped<TLabel>> source)
            {
                return new TimelineObservable<double, TLabel>(key, source);
            }
        }

        class TimelineObservable<TKey, TLabel> : IGroupedObservable<Timestamped<TKey>, Timestamped<TLabel>>
        {
            readonly IObservable<Timestamped<TLabel>> timestamps;

            public TimelineObservable(Timestamped<TKey> key, IObservable<Timestamped<TLabel>> source)
            {
                Key = key;
                timestamps = source;
            }

            public Timestamped<TKey> Key { get; }

            public IDisposable Subscribe(IObserver<Timestamped<TLabel>> observer)
            {
                return timestamps.Subscribe(observer);
            }
        }
    }
}
