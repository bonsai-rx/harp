using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that filters the sequence for Harp messages that
    /// match the specified message type.
    /// </summary>
    [WorkflowElementIcon(typeof(ElementCategory), "Reactive.Condition")]
    [Description("Filters the sequence for Harp messages that match the specified message type.")]
    public class FilterMessageType : Combinator<HarpMessage, HarpMessage>
    {
        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies how the message filter will use the matching criteria.")]
        public FilterType FilterType { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the expected message type. If no value is
        /// specified, all messages will be accepted.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies the expected message type. If no value is specified, all messages will be accepted.")]
        public MessageType? MessageType { get; set; }

        /// <summary>
        /// Filters an observable sequence for Harp messages matching the specified
        /// message type criteria.
        /// </summary>
        /// <param name="source">An observable sequence of Harp messages.</param>
        /// <returns>
        /// An observable sequence including or excluding the Harp messages matching
        /// the specified message type, depending on the specified filter type.
        /// If message type is <see langword="null"/>, messages of any type are accepted.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            var messageType = MessageType;
            var includeMatch = FilterType == FilterType.Include;
            return source.Where(message =>
                !messageType.HasValue ||
                (message.MessageType == messageType.GetValueOrDefault()
                    ? includeMatch
                    : !includeMatch));
        }
    }
}
