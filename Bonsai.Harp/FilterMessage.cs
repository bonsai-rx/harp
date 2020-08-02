using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which filters a sequence of Harp messages for elements that match the specified address and message type.
    /// </summary>
    [WorkflowElementCategory(ElementCategory.Condition)]
    [Description("Filters a sequence of Harp messages for elements that match the specified address and message type.")]
    public class FilterMessage : Combinator<HarpMessage, HarpMessage>
    {
        /// <summary>
        /// Gets or sets the desired message address. This parameter is optional.
        /// </summary>
        [Description("The desired message address. This parameter is optional.")]
        public int? Address { get; set; }

        /// <summary>
        /// Gets or sets the desired type of the message. This parameter is optional.
        /// </summary>
        [Description("The desired type of the message. This parameter is optional.")]
        public MessageType? MessageType { get; set; }

        /// <summary>
        /// Returns an observable sequence of Harp messages matching the specified address and message type.
        /// </summary>
        /// <param name="source">An observable sequence of Harp messages.</param>
        /// <returns>
        /// An observable sequence of Harp messages matching the specified address and message type. If
        /// <c>Address</c> or <c>MessageType</c> are <c>null</c>, any address or message type, respectively,
        /// are accepted.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            var address = Address;
            var messageType = MessageType;
            if (address == null && messageType == null) return source;
            if (address == null) return source.Where(input => input.MessageType == messageType);
            if (messageType == null) return source.Where(input => input.Address == address);
            return source.Where(input => input.Address == address && input.MessageType == messageType);
        }
    }
}
