using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which filters and selects specific event messages
    /// reported by all Harp devices.
    /// </summary>
    [XmlInclude(typeof(Heartbeat))]
    [XmlInclude(typeof(MessageTimestamp))]
    [Description("Filters and selects event messages reported by all Harp devices.")]
    public class DeviceEvent : EventBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceEvent"/> class.
        /// </summary>
        public DeviceEvent()
        {
            Event = new Heartbeat();
        }

        /// <summary>
        /// Gets or sets a value specifying the type of the event message to select.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0612 // Type or member is obsolete
        public DeviceEventType Type
        {
            get
            {
                return Event?.GetType() switch
                {
                    Type type when type == typeof(Heartbeat) => DeviceEventType.Heartbeat,
                    Type type when type == typeof(MessageTimestamp) => DeviceEventType.MessageTimestamp,
                    _ => default,
                };
            }
            set
            {
                Event = value switch
                {
                    DeviceEventType.MessageTimestamp => new MessageTimestamp(),
                    _ => new Heartbeat(),
                };
            }
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }

    /// <summary>
    /// Specifies standard device events available on all Harp devices.
    /// </summary>
    [Obsolete]
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

    /// <summary>
    /// Represents an operator that filters and selects the current time of the device,
    /// reported once every second after synchronizing with the periodic timing signal.
    /// </summary>
    [Description("Filters and selects the current time of the device, reported once every second after synchronizing with the periodic timing signal.")]
    public class Heartbeat : Combinator<HarpMessage, uint>
    {
        /// <summary>
        /// Filters and selects the current time of the device, reported once every
        /// second after synchronizing with the periodic timing signal.
        /// </summary>
        /// <param name="source">The sequence of Harp event messages.</param>
        /// <returns>
        /// A sequence of 32-bit unsigned integers representing the whole
        /// part of the device timestamp, in seconds.
        /// </returns>
        public override IObservable<uint> Process(IObservable<HarpMessage> source)
        {
            return source.Event(DeviceRegisters.TimestampSecond).Select(input => input.GetPayloadUInt32());
        }
    }

    /// <summary>
    /// Represents an operator that selects the timestamp, in seconds, for each
    /// event message in the source sequence.
    /// </summary>
    [Description("Selects the timestamp, in seconds, for each event message in the source sequence.")]
    public class MessageTimestamp : Combinator<HarpMessage, double>
    {
        /// <summary>
        /// Selects the timestamp, in seconds, for each event message in the
        /// source sequence.
        /// </summary>
        /// <param name="source">The sequence of Harp event messages.</param>
        /// <returns>
        /// A sequence of double precision floating-point values representing
        /// the message timestamp, in whole and fractional seconds.
        /// </returns>
        public override IObservable<double> Process(IObservable<HarpMessage> source)
        {
            return source
                .Where(input => input.MessageType == MessageType.Event && input.IsTimestamped)
                .Select(input => input.GetTimestamp());
        }
    }
}
