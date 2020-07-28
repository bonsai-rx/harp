using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp
{
    [Obsolete]
    [WorkflowElementCategory(ElementCategory.Condition)]
    [Description("Filters a sequence of Harp messages for elements that match the specified address and message type.")]
    public class FilterHarpMessage : Combinator<HarpMessage, HarpMessage>
    {
        [Description("The desired message address. This parameter is optional.")]
        public int? Address { get; set; }

        [Description("The desired type of the message. This parameter is optional.")]
        public MessageType? MessageType { get; set; }

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
