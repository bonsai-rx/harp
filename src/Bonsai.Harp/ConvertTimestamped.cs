using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Bonsai.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an expression builder which converts a timestamped payload value
    /// into a new form while keeping the original timestamp.
    /// </summary>
    [Description("Converts a timestamped payload value into a new form while keeping the original timestamp.")]
    public class ConvertTimestamped : WorkflowExpressionBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 1, upperBound: 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertTimestamped"/> class.
        /// </summary>
        public ConvertTimestamped()
            : this(new ExpressionBuilderGraph())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertTimestamped"/> class
        /// with the specified expression builder workflow.
        /// </summary>
        /// <param name="workflow">
        /// The expression builder workflow instance that will be used by this builder
        /// to generate the output expression tree.
        /// </param>
        public ConvertTimestamped(ExpressionBuilderGraph workflow)
            : base(workflow)
        {
        }

        /// <inheritdoc/>
        public override Range<int> ArgumentRange
        {
            get { return argumentRange; }
        }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var source = arguments.FirstOrDefault();
            if (source == null)
            {
                throw new InvalidOperationException($"There must be exactly one workflow input to {nameof(ConvertTimestamped)}.");
            }

            var sourceType = source.Type.GetGenericArguments()[0];
            if (!sourceType.IsGenericType || sourceType.GetGenericTypeDefinition() != typeof(Timestamped<>))
            {
                throw new InvalidOperationException($"The specified input must be of type {typeof(Timestamped<>)}.");
            }

            var timestampedType = sourceType.GetGenericArguments()[0];
            var selectorParameter = Expression.Parameter(typeof(IObservable<>).MakeGenericType(timestampedType));
            return BuildWorkflow(arguments, selectorParameter, selectorBody =>
            {
                var selector = Expression.Lambda(selectorBody, selectorParameter);
                var selectorObservableType = selector.ReturnType.GetGenericArguments()[0];
                return Expression.Call(
                    typeof(ConvertTimestamped),
                    nameof(Process),
                    new[] { timestampedType, selectorObservableType },
                    source,
                    selector);
            });
        }

        static IObservable<Timestamped<TResult>> Process<TSource, TResult>(
            IObservable<Timestamped<TSource>> source,
            Func<IObservable<TSource>, IObservable<TResult>> selector)
        {
            return source.Publish(ps => selector(
                ps.Select(ps => ps.Value)).Zip(ps, (x, y) => Timestamped.Create(x, y.Seconds)));
        }
    }
}
