using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;

namespace Bonsai.Harp
{
    public partial class Device
    {
        /// <summary>
        /// Gets a read-only mapping from address to register name.
        /// </summary>
        public static IReadOnlyDictionary<int, string> RegisterMap { get; } = new Dictionary<int, string>
        {
            { 0, "WhoAmI" },
            { 1, "HardwareVersionHigh" },
            { 2, "HardwareVersionLow" },
            { 3, "AssemblyVersion" },
            { 4, "CoreVersionHigh" },
            { 5, "CoreVersionLow" },
            { 6, "FirmwareVersionHigh" },
            { 7, "FirmwareVersionLow" },
            { 8, "TimestampSeconds" },
            { 9, "TimestampMicroseconds" },
            { 10, "OperationControl" },
            { 11, "ResetDevice" },
            { 12, "DeviceName" },
            { 13, "SerialNumber" },
            { 14, "ClockConfiguration" }
        };
    }

    /// <summary>
    /// Represents an operator that specifies the identity class of the device.
    /// </summary>
    [Description("Specifies the identity class of the device.")]
    public partial class WhoAmI : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="WhoAmI"/> register. This field is constant.
        /// </summary>
        public const int Address = 0;

        /// <summary>
        /// Represents the payload type of the <see cref="WhoAmI"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="WhoAmI"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhoAmI"/> class.
        /// </summary>
        public WhoAmI()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="WhoAmI"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static int GetPayload(HarpMessage message)
        {
            return (int)message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="WhoAmI"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<int> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadUInt16();
            return Timestamped.Create((int)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="WhoAmI"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="WhoAmI"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, int value)
        {
            return HarpMessage.FromUInt16(Address, messageType, (ushort)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="WhoAmI"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="WhoAmI"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, int value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, (ushort)value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="WhoAmI"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="int"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<int> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="WhoAmI"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="int"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="WhoAmI"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<int> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="WhoAmI"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="int"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="WhoAmI"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<int>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the WhoAmI register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the WhoAmI register.")]
    public partial class TimestampedWhoAmI : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="WhoAmI"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="int"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<int>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(WhoAmI.Address, MessageType).Select(WhoAmI.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the major hardware version of the device.
    /// </summary>
    [Description("Specifies the major hardware version of the device.")]
    public partial class HardwareVersionHigh : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="HardwareVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int Address = 1;

        /// <summary>
        /// Represents the payload type of the <see cref="HardwareVersionHigh"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="HardwareVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareVersionHigh"/> class.
        /// </summary>
        public HardwareVersionHigh()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="HardwareVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="HardwareVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="HardwareVersionHigh"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="HardwareVersionHigh"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="HardwareVersionHigh"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="HardwareVersionHigh"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="HardwareVersionHigh"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<byte> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="HardwareVersionHigh"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="HardwareVersionHigh"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<byte> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="HardwareVersionHigh"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="byte"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="HardwareVersionHigh"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<byte>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the HardwareVersionHigh register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the HardwareVersionHigh register.")]
    public partial class TimestampedHardwareVersionHigh : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="HardwareVersionHigh"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="byte"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<byte>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(HardwareVersionHigh.Address, MessageType).Select(HardwareVersionHigh.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the minor hardware version of the device.
    /// </summary>
    [Description("Specifies the minor hardware version of the device.")]
    public partial class HardwareVersionLow : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="HardwareVersionLow"/> register. This field is constant.
        /// </summary>
        public const int Address = 2;

        /// <summary>
        /// Represents the payload type of the <see cref="HardwareVersionLow"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="HardwareVersionLow"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareVersionLow"/> class.
        /// </summary>
        public HardwareVersionLow()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="HardwareVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="HardwareVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="HardwareVersionLow"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="HardwareVersionLow"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="HardwareVersionLow"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="HardwareVersionLow"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="HardwareVersionLow"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<byte> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="HardwareVersionLow"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="HardwareVersionLow"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<byte> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="HardwareVersionLow"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="byte"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="HardwareVersionLow"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<byte>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the HardwareVersionLow register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the HardwareVersionLow register.")]
    public partial class TimestampedHardwareVersionLow : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="HardwareVersionLow"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="byte"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<byte>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(HardwareVersionLow.Address, MessageType).Select(HardwareVersionLow.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the version of the assembled components in the device.
    /// </summary>
    [Description("Specifies the version of the assembled components in the device.")]
    public partial class AssemblyVersion : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="AssemblyVersion"/> register. This field is constant.
        /// </summary>
        public const int Address = 3;

        /// <summary>
        /// Represents the payload type of the <see cref="AssemblyVersion"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="AssemblyVersion"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyVersion"/> class.
        /// </summary>
        public AssemblyVersion()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="AssemblyVersion"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="AssemblyVersion"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="AssemblyVersion"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AssemblyVersion"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="AssemblyVersion"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="AssemblyVersion"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="AssemblyVersion"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<byte> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="AssemblyVersion"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="AssemblyVersion"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<byte> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="AssemblyVersion"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="byte"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="AssemblyVersion"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<byte>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the AssemblyVersion register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the AssemblyVersion register.")]
    public partial class TimestampedAssemblyVersion : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="AssemblyVersion"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="byte"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<byte>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(AssemblyVersion.Address, MessageType).Select(AssemblyVersion.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the major version of the Harp core implemented by the device.
    /// </summary>
    [Description("Specifies the major version of the Harp core implemented by the device.")]
    public partial class CoreVersionHigh : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="CoreVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int Address = 4;

        /// <summary>
        /// Represents the payload type of the <see cref="CoreVersionHigh"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="CoreVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreVersionHigh"/> class.
        /// </summary>
        public CoreVersionHigh()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="CoreVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="CoreVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="CoreVersionHigh"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="CoreVersionHigh"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="CoreVersionHigh"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="CoreVersionHigh"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="CoreVersionHigh"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<byte> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="CoreVersionHigh"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="CoreVersionHigh"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<byte> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="CoreVersionHigh"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="byte"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="CoreVersionHigh"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<byte>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the CoreVersionHigh register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the CoreVersionHigh register.")]
    public partial class TimestampedCoreVersionHigh : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="CoreVersionHigh"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="byte"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<byte>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(CoreVersionHigh.Address, MessageType).Select(CoreVersionHigh.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the minor version of the Harp core implemented by the device.
    /// </summary>
    [Description("Specifies the minor version of the Harp core implemented by the device.")]
    public partial class CoreVersionLow : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="CoreVersionLow"/> register. This field is constant.
        /// </summary>
        public const int Address = 5;

        /// <summary>
        /// Represents the payload type of the <see cref="CoreVersionLow"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="CoreVersionLow"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreVersionLow"/> class.
        /// </summary>
        public CoreVersionLow()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="CoreVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="CoreVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="CoreVersionLow"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="CoreVersionLow"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="CoreVersionLow"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="CoreVersionLow"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="CoreVersionLow"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<byte> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="CoreVersionLow"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="CoreVersionLow"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<byte> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="CoreVersionLow"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="byte"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="CoreVersionLow"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<byte>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the CoreVersionLow register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the CoreVersionLow register.")]
    public partial class TimestampedCoreVersionLow : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="CoreVersionLow"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="byte"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<byte>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(CoreVersionLow.Address, MessageType).Select(CoreVersionLow.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the major version of the Harp core implemented by the device.
    /// </summary>
    [Description("Specifies the major version of the Harp core implemented by the device.")]
    public partial class FirmwareVersionHigh : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="FirmwareVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int Address = 6;

        /// <summary>
        /// Represents the payload type of the <see cref="FirmwareVersionHigh"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="FirmwareVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirmwareVersionHigh"/> class.
        /// </summary>
        public FirmwareVersionHigh()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="FirmwareVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FirmwareVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FirmwareVersionHigh"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FirmwareVersionHigh"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FirmwareVersionHigh"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FirmwareVersionHigh"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="FirmwareVersionHigh"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<byte> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="FirmwareVersionHigh"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="FirmwareVersionHigh"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<byte> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="FirmwareVersionHigh"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="byte"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="FirmwareVersionHigh"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<byte>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the FirmwareVersionHigh register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the FirmwareVersionHigh register.")]
    public partial class TimestampedFirmwareVersionHigh : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="FirmwareVersionHigh"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="byte"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<byte>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(FirmwareVersionHigh.Address, MessageType).Select(FirmwareVersionHigh.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the minor version of the Harp core implemented by the device.
    /// </summary>
    [Description("Specifies the minor version of the Harp core implemented by the device.")]
    public partial class FirmwareVersionLow : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="FirmwareVersionLow"/> register. This field is constant.
        /// </summary>
        public const int Address = 7;

        /// <summary>
        /// Represents the payload type of the <see cref="FirmwareVersionLow"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="FirmwareVersionLow"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirmwareVersionLow"/> class.
        /// </summary>
        public FirmwareVersionLow()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="FirmwareVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static byte GetPayload(HarpMessage message)
        {
            return message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="FirmwareVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadByte();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="FirmwareVersionLow"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FirmwareVersionLow"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="FirmwareVersionLow"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="FirmwareVersionLow"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, byte value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="FirmwareVersionLow"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<byte> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="FirmwareVersionLow"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="byte"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="FirmwareVersionLow"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<byte> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="FirmwareVersionLow"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="byte"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="FirmwareVersionLow"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<byte>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the FirmwareVersionLow register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the FirmwareVersionLow register.")]
    public partial class TimestampedFirmwareVersionLow : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="FirmwareVersionLow"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="byte"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<byte>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(FirmwareVersionLow.Address, MessageType).Select(FirmwareVersionLow.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that stores the integral part of the system timestamp, in seconds.
    /// </summary>
    [Description("Stores the integral part of the system timestamp, in seconds.")]
    public partial class TimestampSeconds : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="TimestampSeconds"/> register. This field is constant.
        /// </summary>
        public const int Address = 8;

        /// <summary>
        /// Represents the payload type of the <see cref="TimestampSeconds"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U32;

        /// <summary>
        /// Represents the length of the <see cref="TimestampSeconds"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimestampSeconds"/> class.
        /// </summary>
        public TimestampSeconds()
        {
            MessageType = MessageType.Event;
        }

        /// <summary>
        /// Returns the payload data for <see cref="TimestampSeconds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static uint GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt32();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TimestampSeconds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt32();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="TimestampSeconds"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TimestampSeconds"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="TimestampSeconds"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TimestampSeconds"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, uint value)
        {
            return HarpMessage.FromUInt32(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="TimestampSeconds"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="uint"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<uint> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="TimestampSeconds"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="uint"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="TimestampSeconds"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<uint> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="TimestampSeconds"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="uint"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="TimestampSeconds"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<uint>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the TimestampSeconds register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the TimestampSeconds register.")]
    public partial class TimestampedTimestampSeconds : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="TimestampSeconds"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="uint"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<uint>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(TimestampSeconds.Address, MessageType).Select(TimestampSeconds.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that stores the fractional part of the system timestamp, in microseconds.
    /// </summary>
    [Description("Stores the fractional part of the system timestamp, in microseconds.")]
    public partial class TimestampMicroseconds : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="TimestampMicroseconds"/> register. This field is constant.
        /// </summary>
        public const int Address = 9;

        /// <summary>
        /// Represents the payload type of the <see cref="TimestampMicroseconds"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="TimestampMicroseconds"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimestampMicroseconds"/> class.
        /// </summary>
        public TimestampMicroseconds()
        {
            MessageType = MessageType.Read;
        }

        /// <summary>
        /// Returns the payload data for <see cref="TimestampMicroseconds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="TimestampMicroseconds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="TimestampMicroseconds"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TimestampMicroseconds"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="TimestampMicroseconds"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="TimestampMicroseconds"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="TimestampMicroseconds"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="ushort"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<ushort> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="TimestampMicroseconds"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="ushort"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="TimestampMicroseconds"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<ushort> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="TimestampMicroseconds"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="ushort"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="TimestampMicroseconds"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<ushort>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the TimestampMicroseconds register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the TimestampMicroseconds register.")]
    public partial class TimestampedTimestampMicroseconds : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="TimestampMicroseconds"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="ushort"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<ushort>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(TimestampMicroseconds.Address, MessageType).Select(TimestampMicroseconds.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that stores the configuration mode of the device.
    /// </summary>
    [Description("Stores the configuration mode of the device.")]
    public partial class OperationControl : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="OperationControl"/> register. This field is constant.
        /// </summary>
        public const int Address = 10;

        /// <summary>
        /// Represents the payload type of the <see cref="OperationControl"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="OperationControl"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        static OperationControlPayload ParsePayload(byte payload)
        {
            OperationControlPayload result;
            result.OperationMode = (OperationMode)(byte)(payload & 0x3);
            result.DumpRegisters = (payload & 0x8) != 0;
            result.MuteReplies = (payload & 0x10) != 0;
            result.VisualIndicators = (LedState)(byte)((payload & 0x20) >> 5);
            result.OperationLed = (LedState)(byte)((payload & 0x40) >> 6);
            result.Heartbeat = (EnableFlag)(byte)((payload & 0x80) >> 7);
            return result;
        }

        static byte FormatPayload(OperationControlPayload value)
        {
            byte result;
            result = (byte)((byte)value.OperationMode & 0x3);
            result |= (byte)(value.DumpRegisters ? 0x8 : 0);
            result |= (byte)(value.MuteReplies ? 0x10 : 0);
            result |= (byte)(((byte)value.VisualIndicators << 5) & 0x20);
            result |= (byte)(((byte)value.OperationLed << 6) & 0x40);
            result |= (byte)(((byte)value.Heartbeat << 7) & 0x80);
            return result;
        }

        /// <summary>
        /// Returns the payload data for <see cref="OperationControl"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static OperationControlPayload GetPayload(HarpMessage message)
        {
            return ParsePayload(message.GetPayloadByte());
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="OperationControl"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<OperationControlPayload> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create(ParsePayload(payload.Value), payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="OperationControl"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OperationControl"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, OperationControlPayload value)
        {
            return HarpMessage.FromByte(Address, messageType, FormatPayload(value));
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="OperationControl"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="OperationControl"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, OperationControlPayload value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, FormatPayload(value));
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="OperationControl"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="OperationControlPayload"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<OperationControlPayload> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="OperationControl"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="OperationControlPayload"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="OperationControl"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<OperationControlPayload> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="OperationControl"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="OperationControlPayload"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="OperationControl"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<OperationControlPayload>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the OperationControl register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the OperationControl register.")]
    public partial class TimestampedOperationControl : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="OperationControl"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="OperationControlPayload"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<OperationControlPayload>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(OperationControl.Address, MessageType).Select(OperationControl.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that resets the device and saves non-volatile registers.
    /// </summary>
    [Description("Resets the device and saves non-volatile registers.")]
    public partial class ResetDevice : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="ResetDevice"/> register. This field is constant.
        /// </summary>
        public const int Address = 11;

        /// <summary>
        /// Represents the payload type of the <see cref="ResetDevice"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ResetDevice"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ResetDevice"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ResetFlags GetPayload(HarpMessage message)
        {
            return (ResetFlags)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ResetDevice"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ResetFlags> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((ResetFlags)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ResetDevice"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ResetDevice"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ResetFlags value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ResetDevice"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ResetDevice"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ResetFlags value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="ResetDevice"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="ResetFlags"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<ResetFlags> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="ResetDevice"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="ResetFlags"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="ResetDevice"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<ResetFlags> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="ResetDevice"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="ResetFlags"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="ResetDevice"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<ResetFlags>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the ResetDevice register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the ResetDevice register.")]
    public partial class TimestampedResetDevice : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="ResetDevice"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="ResetFlags"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<ResetFlags>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(ResetDevice.Address, MessageType).Select(ResetDevice.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that stores the user-specified device name.
    /// </summary>
    [Description("Stores the user-specified device name.")]
    public partial class DeviceName : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="DeviceName"/> register. This field is constant.
        /// </summary>
        public const int Address = 12;

        /// <summary>
        /// Represents the payload type of the <see cref="DeviceName"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="DeviceName"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 25;

        static string ParsePayload(ArraySegment<byte> payload)
        {
            var count = Array.IndexOf(payload.Array, (byte)0, payload.Offset, payload.Count) - payload.Offset;
            return Encoding.ASCII.GetString(payload.Array, payload.Offset, count);
        }

        static ArraySegment<byte> FormatPayload(string value)
        {
            var payload = new byte[RegisterLength];
            Encoding.ASCII.GetBytes(value, 0, Math.Min(value.Length, RegisterLength - 1), payload, 0);
            return new ArraySegment<byte>(payload);
        }

        /// <summary>
        /// Returns the payload data for <see cref="DeviceName"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static string GetPayload(HarpMessage message)
        {
            return ParsePayload(message.GetPayload());
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="DeviceName"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<string> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayload();
            return Timestamped.Create(ParsePayload(payload.Value), payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="DeviceName"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DeviceName"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, string value)
        {
            return HarpMessage.FromPayload(Address, messageType, PayloadType.U8, FormatPayload(value));
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="DeviceName"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="DeviceName"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, string value)
        {
            return HarpMessage.FromPayload(Address, timestamp, messageType, PayloadType.U8, FormatPayload(value));
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="DeviceName"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="string"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<string> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="DeviceName"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="string"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="DeviceName"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<string> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="DeviceName"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="string"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="DeviceName"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<string>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the DeviceName register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the DeviceName register.")]
    public partial class TimestampedDeviceName : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="DeviceName"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="string"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<string>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(DeviceName.Address, MessageType).Select(DeviceName.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the unique serial number of the device.
    /// </summary>
    [Description("Specifies the unique serial number of the device.")]
    public partial class SerialNumber : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="SerialNumber"/> register. This field is constant.
        /// </summary>
        public const int Address = 13;

        /// <summary>
        /// Represents the payload type of the <see cref="SerialNumber"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U16;

        /// <summary>
        /// Represents the length of the <see cref="SerialNumber"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="SerialNumber"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ushort GetPayload(HarpMessage message)
        {
            return message.GetPayloadUInt16();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="SerialNumber"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetTimestampedPayload(HarpMessage message)
        {
            return message.GetTimestampedPayloadUInt16();
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="SerialNumber"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SerialNumber"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, messageType, value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="SerialNumber"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="SerialNumber"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ushort value)
        {
            return HarpMessage.FromUInt16(Address, timestamp, messageType, value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="SerialNumber"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="ushort"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<ushort> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="SerialNumber"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="ushort"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="SerialNumber"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<ushort> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="SerialNumber"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="ushort"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="SerialNumber"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<ushort>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the SerialNumber register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the SerialNumber register.")]
    public partial class TimestampedSerialNumber : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="SerialNumber"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="ushort"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<ushort>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(SerialNumber.Address, MessageType).Select(SerialNumber.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that specifies the configuration for the device synchronization clock.
    /// </summary>
    [Description("Specifies the configuration for the device synchronization clock.")]
    public partial class ClockConfiguration : HarpCombinator
    {
        /// <summary>
        /// Represents the address of the <see cref="ClockConfiguration"/> register. This field is constant.
        /// </summary>
        public const int Address = 14;

        /// <summary>
        /// Represents the payload type of the <see cref="ClockConfiguration"/> register. This field is constant.
        /// </summary>
        public const PayloadType RegisterType = PayloadType.U8;

        /// <summary>
        /// Represents the length of the <see cref="ClockConfiguration"/> register. This field is constant.
        /// </summary>
        public const int RegisterLength = 1;

        /// <summary>
        /// Returns the payload data for <see cref="ClockConfiguration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the message payload.</returns>
        public static ClockConfigurationFlags GetPayload(HarpMessage message)
        {
            return (ClockConfigurationFlags)message.GetPayloadByte();
        }

        /// <summary>
        /// Returns the timestamped payload data for <see cref="ClockConfiguration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ClockConfigurationFlags> GetTimestampedPayload(HarpMessage message)
        {
            var payload = message.GetTimestampedPayloadByte();
            return Timestamped.Create((ClockConfigurationFlags)payload.Value, payload.Seconds);
        }

        /// <summary>
        /// Returns a Harp message for the <see cref="ClockConfiguration"/> register.
        /// </summary>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ClockConfiguration"/> register
        /// with the specified message type and payload.
        /// </returns>
        public static HarpMessage FromPayload(MessageType messageType, ClockConfigurationFlags value)
        {
            return HarpMessage.FromByte(Address, messageType, (byte)value);
        }

        /// <summary>
        /// Returns a timestamped Harp message for the <see cref="ClockConfiguration"/>
        /// register.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">The type of the Harp message.</param>
        /// <param name="value">The value to be stored in the message payload.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object for the <see cref="ClockConfiguration"/> register
        /// with the specified message type, timestamp, and payload.
        /// </returns>
        public static HarpMessage FromPayload(double timestamp, MessageType messageType, ClockConfigurationFlags value)
        {
            return HarpMessage.FromByte(Address, timestamp, messageType, (byte)value);
        }

        /// <summary>
        /// Filters and selects an observable sequence of messages from the
        /// <see cref="ClockConfiguration"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of <see cref="ClockConfigurationFlags"/> objects representing the
        /// message payload.
        /// </returns>
        public IObservable<ClockConfigurationFlags> Process(IObservable<HarpMessage> source)
        {
            return source.Where(Address, MessageType).Select(GetPayload);
        }

        /// <summary>
        /// Formats an observable sequence of values into Harp messages
        /// for the <see cref="ClockConfiguration"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of <see cref="ClockConfigurationFlags"/> objects representing the
        /// message payload.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects formatted for the
        /// <see cref="ClockConfiguration"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<ClockConfigurationFlags> source)
        {
            return source.Select(value => FromPayload(MessageType, value));
        }

        /// <summary>
        /// Formats an observable sequence of values into timestamped Harp messages
        /// for the <see cref="ClockConfiguration"/> register.
        /// </summary>
        /// <param name="source">
        /// A sequence of timestamped <see cref="ClockConfigurationFlags"/> objects representing
        /// the message payload.
        /// </param>
        /// <returns>
        /// A sequence of timestamped <see cref="HarpMessage"/> objects formatted for
        /// the <see cref="ClockConfiguration"/> register.
        /// </returns>
        public IObservable<HarpMessage> Process(IObservable<Timestamped<ClockConfigurationFlags>> source)
        {
            return source.Select(payload => FromPayload(payload.Seconds, MessageType, payload.Value));
        }
    }

    /// <summary>
    /// Represents an operator that filters and selects a sequence of timestamped messages
    /// from the ClockConfiguration register.
    /// </summary>
    [Description("Filters and selects timestamped messages from the ClockConfiguration register.")]
    public partial class TimestampedClockConfiguration : HarpCombinator
    {
        /// <summary>
        /// Filters and selects an observable sequence of timestamped messages from
        /// the <see cref="ClockConfiguration"/> register.
        /// </summary>
        /// <param name="source">The sequence of Harp device messages.</param>
        /// <returns>
        /// A sequence of timestamped <see cref="ClockConfigurationFlags"/> objects
        /// representing the register payload.
        /// </returns>
        public IObservable<Timestamped<ClockConfigurationFlags>> Process(IObservable<HarpMessage> source)
        {
            return source.Where(ClockConfiguration.Address, MessageType).Select(ClockConfiguration.GetTimestampedPayload);
        }
    }

    /// <summary>
    /// Represents an operator that creates a sequence of message payloads
    /// that stores the integral part of the system timestamp, in seconds.
    /// </summary>
    [DisplayName("TimestampSecondsPayload")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Creates a sequence of message payloads that stores the integral part of the system timestamp, in seconds.")]
    public partial class CreateTimestampSecondsPayload : HarpCombinator
    {
        /// <summary>
        /// Gets or sets the value that stores the integral part of the system timestamp, in seconds.
        /// </summary>
        [Description("The value that stores the integral part of the system timestamp, in seconds.")]
        public uint Value { get; set; }

        /// <summary>
        /// Creates an observable sequence that contains a single message
        /// that stores the integral part of the system timestamp, in seconds.
        /// </summary>
        /// <returns>
        /// A sequence containing a single <see cref="HarpMessage"/> object
        /// representing the created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process()
        {
            return Process(Observable.Return(System.Reactive.Unit.Default));
        }

        /// <summary>
        /// Creates an observable sequence of message payloads
        /// that stores the integral part of the system timestamp, in seconds.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting message payloads.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing each
        /// created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => TimestampSeconds.FromPayload(MessageType, Value));
        }
    }

    /// <summary>
    /// Represents an operator that creates a sequence of message payloads
    /// that stores the configuration mode of the device.
    /// </summary>
    [DisplayName("OperationControlPayload")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Creates a sequence of message payloads that stores the configuration mode of the device.")]
    public partial class CreateOperationControlPayload : HarpCombinator
    {
        /// <summary>
        /// Gets or sets a value that specifies the operation mode of the device.
        /// </summary>
        [Description("Specifies the operation mode of the device.")]
        public OperationMode OperationMode { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether the device should report the content of all registers on initialization.
        /// </summary>
        [Description("Specifies whether the device should report the content of all registers on initialization.")]
        public bool DumpRegisters { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether the replies to all commands will be muted, i.e. not sent by the device.
        /// </summary>
        [Description("Specifies whether the replies to all commands will be muted, i.e. not sent by the device.")]
        public bool MuteReplies { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the state of all visual indicators on the device.
        /// </summary>
        [Description("Specifies the state of all visual indicators on the device.")]
        public LedState VisualIndicators { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether the device state LED should report the operation mode of the device.
        /// </summary>
        [Description("Specifies whether the device state LED should report the operation mode of the device.")]
        public LedState OperationLed { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether the device should report the content of the seconds register each second.
        /// </summary>
        [Description("Specifies whether the device should report the content of the seconds register each second.")]
        public EnableFlag Heartbeat { get; set; }

        /// <summary>
        /// Creates an observable sequence that contains a single message
        /// that stores the configuration mode of the device.
        /// </summary>
        /// <returns>
        /// A sequence containing a single <see cref="HarpMessage"/> object
        /// representing the created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process()
        {
            return Process(Observable.Return(System.Reactive.Unit.Default));
        }

        /// <summary>
        /// Creates an observable sequence of message payloads
        /// that stores the configuration mode of the device.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting message payloads.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing each
        /// created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ =>
            {
                OperationControlPayload value;
                value.OperationMode = OperationMode;
                value.DumpRegisters = DumpRegisters;
                value.MuteReplies = MuteReplies;
                value.VisualIndicators = VisualIndicators;
                value.OperationLed = OperationLed;
                value.Heartbeat = Heartbeat;
                return OperationControl.FromPayload(MessageType, value);
            });
        }
    }

    /// <summary>
    /// Represents an operator that creates a sequence of message payloads
    /// that resets the device and saves non-volatile registers.
    /// </summary>
    [DisplayName("ResetDevicePayload")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Creates a sequence of message payloads that resets the device and saves non-volatile registers.")]
    public partial class CreateResetDevicePayload : HarpCombinator
    {
        /// <summary>
        /// Gets or sets the value that resets the device and saves non-volatile registers.
        /// </summary>
        [Description("The value that resets the device and saves non-volatile registers.")]
        public ResetFlags Value { get; set; }

        /// <summary>
        /// Creates an observable sequence that contains a single message
        /// that resets the device and saves non-volatile registers.
        /// </summary>
        /// <returns>
        /// A sequence containing a single <see cref="HarpMessage"/> object
        /// representing the created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process()
        {
            return Process(Observable.Return(System.Reactive.Unit.Default));
        }

        /// <summary>
        /// Creates an observable sequence of message payloads
        /// that resets the device and saves non-volatile registers.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting message payloads.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing each
        /// created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => ResetDevice.FromPayload(MessageType, Value));
        }
    }

    /// <summary>
    /// Represents an operator that creates a sequence of message payloads
    /// that stores the user-specified device name.
    /// </summary>
    [DisplayName("DeviceNamePayload")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Creates a sequence of message payloads that stores the user-specified device name.")]
    public partial class CreateDeviceNamePayload : HarpCombinator
    {
        /// <summary>
        /// Gets or sets the value that stores the user-specified device name.
        /// </summary>
        [Description("The value that stores the user-specified device name.")]
        public string Value { get; set; }

        /// <summary>
        /// Creates an observable sequence that contains a single message
        /// that stores the user-specified device name.
        /// </summary>
        /// <returns>
        /// A sequence containing a single <see cref="HarpMessage"/> object
        /// representing the created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process()
        {
            return Process(Observable.Return(System.Reactive.Unit.Default));
        }

        /// <summary>
        /// Creates an observable sequence of message payloads
        /// that stores the user-specified device name.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting message payloads.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing each
        /// created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => DeviceName.FromPayload(MessageType, Value));
        }
    }

    /// <summary>
    /// Represents an operator that creates a sequence of message payloads
    /// that specifies the unique serial number of the device.
    /// </summary>
    [DisplayName("SerialNumberPayload")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Creates a sequence of message payloads that specifies the unique serial number of the device.")]
    public partial class CreateSerialNumberPayload : HarpCombinator
    {
        /// <summary>
        /// Gets or sets the value that specifies the unique serial number of the device.
        /// </summary>
        [Description("The value that specifies the unique serial number of the device.")]
        public ushort Value { get; set; }

        /// <summary>
        /// Creates an observable sequence that contains a single message
        /// that specifies the unique serial number of the device.
        /// </summary>
        /// <returns>
        /// A sequence containing a single <see cref="HarpMessage"/> object
        /// representing the created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process()
        {
            return Process(Observable.Return(System.Reactive.Unit.Default));
        }

        /// <summary>
        /// Creates an observable sequence of message payloads
        /// that specifies the unique serial number of the device.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting message payloads.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing each
        /// created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => SerialNumber.FromPayload(MessageType, Value));
        }
    }

    /// <summary>
    /// Represents an operator that creates a sequence of message payloads
    /// that specifies the configuration for the device synchronization clock.
    /// </summary>
    [DisplayName("ClockConfigurationPayload")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Creates a sequence of message payloads that specifies the configuration for the device synchronization clock.")]
    public partial class CreateClockConfigurationPayload : HarpCombinator
    {
        /// <summary>
        /// Gets or sets the value that specifies the configuration for the device synchronization clock.
        /// </summary>
        [Description("The value that specifies the configuration for the device synchronization clock.")]
        public ClockConfigurationFlags Value { get; set; }

        /// <summary>
        /// Creates an observable sequence that contains a single message
        /// that specifies the configuration for the device synchronization clock.
        /// </summary>
        /// <returns>
        /// A sequence containing a single <see cref="HarpMessage"/> object
        /// representing the created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process()
        {
            return Process(Observable.Return(System.Reactive.Unit.Default));
        }

        /// <summary>
        /// Creates an observable sequence of message payloads
        /// that specifies the configuration for the device synchronization clock.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting message payloads.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing each
        /// created message payload.
        /// </returns>
        public IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => ClockConfiguration.FromPayload(MessageType, Value));
        }
    }

    /// <summary>
    /// Represents the payload of the OperationControl register.
    /// </summary>
    public struct OperationControlPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationControlPayload"/> structure.
        /// </summary>
        /// <param name="operationMode">Specifies the operation mode of the device.</param>
        /// <param name="dumpRegisters">Specifies whether the device should report the content of all registers on initialization.</param>
        /// <param name="muteReplies">Specifies whether the replies to all commands will be muted, i.e. not sent by the device.</param>
        /// <param name="visualIndicators">Specifies the state of all visual indicators on the device.</param>
        /// <param name="operationLed">Specifies whether the device state LED should report the operation mode of the device.</param>
        /// <param name="heartbeat">Specifies whether the device should report the content of the seconds register each second.</param>
        public OperationControlPayload(
            OperationMode operationMode,
            bool dumpRegisters,
            bool muteReplies,
            LedState visualIndicators,
            LedState operationLed,
            EnableFlag heartbeat)
        {
            OperationMode = operationMode;
            DumpRegisters = dumpRegisters;
            MuteReplies = muteReplies;
            VisualIndicators = visualIndicators;
            OperationLed = operationLed;
            Heartbeat = heartbeat;
        }

        /// <summary>
        /// Specifies the operation mode of the device.
        /// </summary>
        public OperationMode OperationMode;

        /// <summary>
        /// Specifies whether the device should report the content of all registers on initialization.
        /// </summary>
        public bool DumpRegisters;

        /// <summary>
        /// Specifies whether the replies to all commands will be muted, i.e. not sent by the device.
        /// </summary>
        public bool MuteReplies;

        /// <summary>
        /// Specifies the state of all visual indicators on the device.
        /// </summary>
        public LedState VisualIndicators;

        /// <summary>
        /// Specifies whether the device state LED should report the operation mode of the device.
        /// </summary>
        public LedState OperationLed;

        /// <summary>
        /// Specifies whether the device should report the content of the seconds register each second.
        /// </summary>
        public EnableFlag Heartbeat;
    }

    /// <summary>
    /// Specifies the behavior of the non-volatile registers when resetting the device.
    /// </summary>
    [Flags]
    public enum ResetFlags : byte
    {
        /// <summary>
        /// All reset flags are cleared.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// The device will boot with all the registers reset to their default factory values.
        /// </summary>
        RestoreDefault = 0x1,

        /// <summary>
        /// The device will boot and restore all the registers to the values stored in non-volatile memory.
        /// </summary>
        RestoreEeprom = 0x2,

        /// <summary>
        /// The device will boot and save all the current register values to non-volatile memory.
        /// </summary>
        Save = 0x4,

        /// <summary>
        /// The device will boot with the default device name.
        /// </summary>
        RestoreName = 0x8,

        /// <summary>
        /// Specifies that the device has booted from default factory values.
        /// </summary>
        BootFromDefault = 0x40,

        /// <summary>
        /// Specifies that the device has booted from non-volatile values stored in EEPROM.
        /// </summary>
        BootFromEeprom = 0x80
    }

    /// <summary>
    /// Specifies configuration flags for the device synchronization clock.
    /// </summary>
    [Flags]
    public enum ClockConfigurationFlags : byte
    {
        /// <summary>
        /// All clock configuration flags are cleared.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// The device will repeat the clock synchronization signal to the clock output connector, if available.
        /// </summary>
        ClockRepeater = 0x1,

        /// <summary>
        /// The device resets and generates the clock synchronization signal on the clock output connector, if available.
        /// </summary>
        ClockGenerator = 0x2,

        /// <summary>
        /// Specifies the device has the capability to repeat the clock synchronization signal to the clock output connector.
        /// </summary>
        RepeaterCapability = 0x8,

        /// <summary>
        /// Specifies the device has the capability to generate the clock synchronization signal to the clock output connector.
        /// </summary>
        GeneratorCapability = 0x10,

        /// <summary>
        /// The device will unlock the timestamp register counter and will accept commands to set new timestamp values.
        /// </summary>
        ClockUnlock = 0x40,

        /// <summary>
        /// The device will lock the timestamp register counter and will not accept commands to set new timestamp values.
        /// </summary>
        ClockLock = 0x80
    }

    /// <summary>
    /// Specifies the operation mode of the device.
    /// </summary>
    public enum OperationMode : byte
    {
        /// <summary>
        /// Disable all event reporting on the device.
        /// </summary>
        Standby = 0,

        /// <summary>
        /// Event detection is enabled. Only enabled events are reported by the device.
        /// </summary>
        Active = 1,

        /// <summary>
        /// The device enters speed mode.
        /// </summary>
        Speed = 3
    }

    /// <summary>
    /// Specifies whether a specific register flag is enabled or disabled.
    /// </summary>
    public enum EnableFlag : byte
    {
        /// <summary>
        /// Specifies that the flag is disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Specifies that the flag is enabled.
        /// </summary>
        Enabled = 1,

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Disable = Disabled,

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Enable = Enabled
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
