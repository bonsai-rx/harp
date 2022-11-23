using System;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for an operator which formats new Harp
    /// command messages.
    /// </summary>
    [WorkflowElementCategory(ElementCategory.Transform)]
    public abstract class CommandFormatter : Combinator<HarpMessage>
    {
        /// <summary>
        /// When overridden in a derived class, formats a new Harp command message.
        /// </summary>
        /// <returns>
        /// A new <see cref="HarpMessage"/> object representing the Harp command.
        /// </returns>
        protected abstract HarpMessage Format();

        /// <inheritdoc/>
        public override IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(input => Format());
        }
    }

    /// <summary>
    /// Provides the abstract base class for an operator which formats input data
    /// as a Harp command message.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the elements in the source sequence.
    /// </typeparam>
    public abstract class CommandFormatter<TSource> : Transform<TSource, HarpMessage>
    {
        /// <summary>
        /// When overridden in a derived class, formats a new Harp command message
        /// from the specified input value.
        /// </summary>
        /// <param name="value">The input value used to create the Harp command.</param>
        /// <returns>
        /// A new <see cref="HarpMessage"/> object representing the Harp command.
        /// </returns>
        protected abstract HarpMessage Format(TSource value);

        /// <inheritdoc/>
        public override IObservable<HarpMessage> Process(IObservable<TSource> source)
        {
            return source.Select(Format);
        }
    }
}
