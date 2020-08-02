using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which filters a sequence of Harp messages for elements that match the specified address and message type.
    /// </summary>
    [Obsolete]
    [WorkflowElementCategory(ElementCategory.Condition)]
    [Description("Filters a sequence of Harp messages for elements that match the specified address and message type.")]
    public class FilterHarpMessage : Combinator<HarpMessage, HarpMessage>
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
            return source.Where(input =>
            {
                var address = Address;
                var messageType = MessageType;
                if ((address == null) && (messageType == null))
                {
                    return true;
                }

                if ((address != null) && (messageType == null))
                {
                    return (address == input.Address);
                }

                if ((address == null) && (messageType != null))
                {
                    return (messageType == input.MessageType);
                }

                if ((address != null) && (messageType != null))
                {
                    return (address == input.Address) && (messageType == input.MessageType);
                }

                return false;
            });
        }
    }
}
