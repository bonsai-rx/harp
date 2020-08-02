using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which filters and selects specific event messages reported by all Harp devices
    /// </summary>
    [Description("Filters and selects event messages reported by all Harp devices.")]
    [TypeDescriptionProvider(typeof(DeviceTypeDescriptionProvider<DeviceEvent>))]
    public class DeviceEvent : SingleArgumentExpressionBuilder, INamedElement
    {
        /// <summary>
        /// Gets or sets the type of the device event message to select.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("The type of the device event message to select.")]
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

        /// <summary>
        /// Returns the expression that specifies how standard event messages are filtered and selected.
        /// </summary>
        /// <param name="arguments">
        /// A collection of <see cref="Expression"/> nodes that represents the input arguments.
        /// </param>
        /// <returns>
        /// The <see cref="Expression"/> that maps the single input argument to the
        /// contents of the standard event message.
        /// </returns>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var expression = arguments.First();
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

    /// <summary>
    /// Specifies standard device events available on all Harp devices.
    /// </summary>
    public enum DeviceEventType : byte
    {
        /// <summary>
        /// The periodic timing signal, reported once every second, used to synchronize Harp devices.
        /// </summary>
        Heartbeat = 0,

        /// <summary>
        /// Specifies that the timestamp, in seconds, should be selected for each input event.
        /// </summary>
        MessageTimestamp
    }
}
