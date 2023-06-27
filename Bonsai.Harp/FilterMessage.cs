using System;
using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that filters the sequence for Harp messages matching
    /// the specified address and message type.
    /// </summary>
    [Obsolete]
    [WorkflowElementCategory(ElementCategory.Condition)]
    [Description("Filters the sequence for Harp messages matching the specified address and message type.")]
    public class FilterMessage : Combinator<HarpMessage, HarpMessage>
    {
        /// <summary>
        /// Gets or sets a value specifying the expected message type. This parameter is optional.
        /// </summary>
        [Description("Specifies the expected message type. This parameter is optional.")]
        public MessageType? MessageType { get; set; }

        /// <summary>
        /// Gets or sets the expected message address. This parameter is optional.
        /// </summary>
        [Description("The expected message address. This parameter is optional.")]
        public int? Address { get; set; }

        /// <summary>
        /// Returns an observable sequence of Harp messages matching the specified
        /// address and message type.
        /// </summary>
        /// <param name="source">An observable sequence of Harp messages.</param>
        /// <returns>
        /// An observable sequence of Harp messages matching the specified address
        /// and message type. If message type or address are <see langword="null"/>,
        /// messages of any type or from any address, respectively, are accepted.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            var address = Address;
            var messageType = MessageType;
            if (address == null && messageType == null) return source;
            if (address == null) return source.Where(messageType.Value);
            if (messageType == null) return source.Where(address.Value);
            return source.Where(address.Value, messageType.Value);
        }
    }
}
