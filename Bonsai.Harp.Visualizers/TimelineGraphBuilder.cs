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
    [TypeVisualizer(typeof(TimelineGraphVisualizer))]
    [Description("A visualizer that plots each Harp message in the sequence in a synchronized rolling graph.")]
    public class TimelineGraphBuilder : SingleArgumentExpressionBuilder
    {
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
            internal ReplaySubject<IObservable<HarpMessage>> Registers;
        }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var source = arguments.First();
            var parameterType = source.Type.GetGenericArguments()[0];
            Controller = new VisualizerController
            {
                TimeSpan = TimeSpan,
                Registers = new ReplaySubject<IObservable<HarpMessage>>()
            };
            var combinator = Expression.Constant(this);
            return Expression.Call(combinator, nameof(Process), null, source);
        }

        IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            return source.Publish(ps => ps.Merge(
                Process(ps.GroupBy(message => message.Address))
                .IgnoreElements()
                .Select(_ => default(HarpMessage))));
        }

        IObservable<IGroupedObservable<int, HarpMessage>> Process(IObservable<IGroupedObservable<int, HarpMessage>> source)
        {
            return source.Do(Controller.Registers.OnNext);
        }

        IObservable<IGroupedObservable<Type, HarpMessage>> Process(IObservable<IGroupedObservable<Type, HarpMessage>> source)
        {
            return source.Do(Controller.Registers.OnNext);
        }
    }
}
