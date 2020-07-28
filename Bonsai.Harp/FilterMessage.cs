using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp
{
    [WorkflowElementCategory(ElementCategory.Condition)]
    [Description("Filters a sequence of Harp messages that match the specified address and message type.")]
    public class FilterMessage : Combinator<HarpMessage, HarpMessage>
    {
        [Description("The desired message address. This parameter is optional.")]
        public int? Address { get; set; }

        [Description("The desired type of the message. This parameter is optional.")]
        public MessageType? MessageType { get; set; }

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
