using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp
{
    [Description("Selects event data available to all Harp devices.")]
    [TypeDescriptionProvider(typeof(DeviceTypeDescriptionProvider<DeviceEvent>))]
    public class DeviceEvent : SingleArgumentExpressionBuilder, INamedElement
    {
        [RefreshProperties(RefreshProperties.All)]
        public DeviceEventType Type { get; set; } = DeviceEventType.Heartbeat;

        string INamedElement.Name => $"Device.{Type}";

        string Description
        {
            get
            {
                switch (Type)
                {
                    case DeviceEventType.Heartbeat: return "The periodic timing signal, reported once every second, used to synchronize Harp devices.";
                    case DeviceEventType.MessageTimestamp: return "Gets the timestamp, in seconds, for each input event.";
                    default: return null;
                }
            }
        }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                case DeviceEventType.Heartbeat:
                    return Expression.Call(typeof(DeviceEvent), nameof(Heartbeat), null, expression);
                case DeviceEventType.MessageTimestamp:
                    return Expression.Call(typeof(DeviceEvent), nameof(MessageTimestamp), null, expression);
                default:
                    throw new InvalidOperationException("Invalid selection or not supported yet.");
            }
        }

        static IObservable<uint> Heartbeat(IObservable<HarpMessage> source)
        {
            return source
                .Where(input => input.IsMatch(Registers.TimestampSecond, MessageType.Event, PayloadType.TimestampedU32))
                .Select(input => input.GetPayloadUInt32());
        }

        static IObservable<double> MessageTimestamp(IObservable<HarpMessage> source)
        {
            return source
                .Where(input => input.MessageType == MessageType.Event && input.IsTimestamped)
                .Select(input => input.GetTimestamp());
        }
    }

    public enum DeviceEventType : byte
    {
        Heartbeat = 0,
        MessageTimestamp
    }
}
