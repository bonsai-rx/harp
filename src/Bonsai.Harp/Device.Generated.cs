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
        /// Gets a read-only mapping from address to register type.
        /// </summary>
        public static IReadOnlyDictionary<int, Type> RegisterMap { get; } = new Dictionary<int, Type>
        {
            { 0, typeof(WhoAmI) },
            { 1, typeof(HardwareVersionHigh) },
            { 2, typeof(HardwareVersionLow) },
            { 3, typeof(AssemblyVersion) },
            { 4, typeof(CoreVersionHigh) },
            { 5, typeof(CoreVersionLow) },
            { 6, typeof(FirmwareVersionHigh) },
            { 7, typeof(FirmwareVersionLow) },
            { 8, typeof(TimestampSeconds) },
            { 9, typeof(TimestampMicroseconds) },
            { 10, typeof(OperationControl) },
            { 11, typeof(ResetDevice) },
            { 12, typeof(DeviceName) },
            { 13, typeof(SerialNumber) },
            { 14, typeof(ClockConfiguration) }
        };
    }

    /// <summary>
    /// Represents a register that specifies the identity class of the device.
    /// </summary>
    [Description("Specifies the identity class of the device.")]
    public partial class WhoAmI
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// WhoAmI register.
    /// </summary>
    /// <seealso cref="WhoAmI"/>
    [Description("Filters and selects timestamped messages from the WhoAmI register.")]
    public partial class TimestampedWhoAmI
    {
        /// <summary>
        /// Represents the address of the <see cref="WhoAmI"/> register. This field is constant.
        /// </summary>
        public const int Address = WhoAmI.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="WhoAmI"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<int> GetPayload(HarpMessage message)
        {
            return WhoAmI.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the major hardware version of the device.
    /// </summary>
    [Description("Specifies the major hardware version of the device.")]
    public partial class HardwareVersionHigh
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// HardwareVersionHigh register.
    /// </summary>
    /// <seealso cref="HardwareVersionHigh"/>
    [Description("Filters and selects timestamped messages from the HardwareVersionHigh register.")]
    public partial class TimestampedHardwareVersionHigh
    {
        /// <summary>
        /// Represents the address of the <see cref="HardwareVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int Address = HardwareVersionHigh.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="HardwareVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return HardwareVersionHigh.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the minor hardware version of the device.
    /// </summary>
    [Description("Specifies the minor hardware version of the device.")]
    public partial class HardwareVersionLow
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// HardwareVersionLow register.
    /// </summary>
    /// <seealso cref="HardwareVersionLow"/>
    [Description("Filters and selects timestamped messages from the HardwareVersionLow register.")]
    public partial class TimestampedHardwareVersionLow
    {
        /// <summary>
        /// Represents the address of the <see cref="HardwareVersionLow"/> register. This field is constant.
        /// </summary>
        public const int Address = HardwareVersionLow.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="HardwareVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return HardwareVersionLow.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the version of the assembled components in the device.
    /// </summary>
    [Description("Specifies the version of the assembled components in the device.")]
    public partial class AssemblyVersion
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// AssemblyVersion register.
    /// </summary>
    /// <seealso cref="AssemblyVersion"/>
    [Description("Filters and selects timestamped messages from the AssemblyVersion register.")]
    public partial class TimestampedAssemblyVersion
    {
        /// <summary>
        /// Represents the address of the <see cref="AssemblyVersion"/> register. This field is constant.
        /// </summary>
        public const int Address = AssemblyVersion.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="AssemblyVersion"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return AssemblyVersion.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the major version of the Harp core implemented by the device.
    /// </summary>
    [Description("Specifies the major version of the Harp core implemented by the device.")]
    public partial class CoreVersionHigh
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// CoreVersionHigh register.
    /// </summary>
    /// <seealso cref="CoreVersionHigh"/>
    [Description("Filters and selects timestamped messages from the CoreVersionHigh register.")]
    public partial class TimestampedCoreVersionHigh
    {
        /// <summary>
        /// Represents the address of the <see cref="CoreVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int Address = CoreVersionHigh.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="CoreVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return CoreVersionHigh.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the minor version of the Harp core implemented by the device.
    /// </summary>
    [Description("Specifies the minor version of the Harp core implemented by the device.")]
    public partial class CoreVersionLow
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// CoreVersionLow register.
    /// </summary>
    /// <seealso cref="CoreVersionLow"/>
    [Description("Filters and selects timestamped messages from the CoreVersionLow register.")]
    public partial class TimestampedCoreVersionLow
    {
        /// <summary>
        /// Represents the address of the <see cref="CoreVersionLow"/> register. This field is constant.
        /// </summary>
        public const int Address = CoreVersionLow.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="CoreVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return CoreVersionLow.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the major version of the Harp core implemented by the device.
    /// </summary>
    [Description("Specifies the major version of the Harp core implemented by the device.")]
    public partial class FirmwareVersionHigh
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FirmwareVersionHigh register.
    /// </summary>
    /// <seealso cref="FirmwareVersionHigh"/>
    [Description("Filters and selects timestamped messages from the FirmwareVersionHigh register.")]
    public partial class TimestampedFirmwareVersionHigh
    {
        /// <summary>
        /// Represents the address of the <see cref="FirmwareVersionHigh"/> register. This field is constant.
        /// </summary>
        public const int Address = FirmwareVersionHigh.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FirmwareVersionHigh"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return FirmwareVersionHigh.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the minor version of the Harp core implemented by the device.
    /// </summary>
    [Description("Specifies the minor version of the Harp core implemented by the device.")]
    public partial class FirmwareVersionLow
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// FirmwareVersionLow register.
    /// </summary>
    /// <seealso cref="FirmwareVersionLow"/>
    [Description("Filters and selects timestamped messages from the FirmwareVersionLow register.")]
    public partial class TimestampedFirmwareVersionLow
    {
        /// <summary>
        /// Represents the address of the <see cref="FirmwareVersionLow"/> register. This field is constant.
        /// </summary>
        public const int Address = FirmwareVersionLow.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="FirmwareVersionLow"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<byte> GetPayload(HarpMessage message)
        {
            return FirmwareVersionLow.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that stores the integral part of the system timestamp, in seconds.
    /// </summary>
    [Description("Stores the integral part of the system timestamp, in seconds.")]
    public partial class TimestampSeconds
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// TimestampSeconds register.
    /// </summary>
    /// <seealso cref="TimestampSeconds"/>
    [Description("Filters and selects timestamped messages from the TimestampSeconds register.")]
    public partial class TimestampedTimestampSeconds
    {
        /// <summary>
        /// Represents the address of the <see cref="TimestampSeconds"/> register. This field is constant.
        /// </summary>
        public const int Address = TimestampSeconds.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="TimestampSeconds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<uint> GetPayload(HarpMessage message)
        {
            return TimestampSeconds.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that stores the fractional part of the system timestamp, in microseconds.
    /// </summary>
    [Description("Stores the fractional part of the system timestamp, in microseconds.")]
    public partial class TimestampMicroseconds
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// TimestampMicroseconds register.
    /// </summary>
    /// <seealso cref="TimestampMicroseconds"/>
    [Description("Filters and selects timestamped messages from the TimestampMicroseconds register.")]
    public partial class TimestampedTimestampMicroseconds
    {
        /// <summary>
        /// Represents the address of the <see cref="TimestampMicroseconds"/> register. This field is constant.
        /// </summary>
        public const int Address = TimestampMicroseconds.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="TimestampMicroseconds"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return TimestampMicroseconds.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that stores the configuration mode of the device.
    /// </summary>
    [Description("Stores the configuration mode of the device.")]
    public partial class OperationControl
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// OperationControl register.
    /// </summary>
    /// <seealso cref="OperationControl"/>
    [Description("Filters and selects timestamped messages from the OperationControl register.")]
    public partial class TimestampedOperationControl
    {
        /// <summary>
        /// Represents the address of the <see cref="OperationControl"/> register. This field is constant.
        /// </summary>
        public const int Address = OperationControl.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="OperationControl"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<OperationControlPayload> GetPayload(HarpMessage message)
        {
            return OperationControl.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that resets the device and saves non-volatile registers.
    /// </summary>
    [Description("Resets the device and saves non-volatile registers.")]
    public partial class ResetDevice
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ResetDevice register.
    /// </summary>
    /// <seealso cref="ResetDevice"/>
    [Description("Filters and selects timestamped messages from the ResetDevice register.")]
    public partial class TimestampedResetDevice
    {
        /// <summary>
        /// Represents the address of the <see cref="ResetDevice"/> register. This field is constant.
        /// </summary>
        public const int Address = ResetDevice.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ResetDevice"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ResetFlags> GetPayload(HarpMessage message)
        {
            return ResetDevice.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that stores the user-specified device name.
    /// </summary>
    [Description("Stores the user-specified device name.")]
    public partial class DeviceName
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// DeviceName register.
    /// </summary>
    /// <seealso cref="DeviceName"/>
    [Description("Filters and selects timestamped messages from the DeviceName register.")]
    public partial class TimestampedDeviceName
    {
        /// <summary>
        /// Represents the address of the <see cref="DeviceName"/> register. This field is constant.
        /// </summary>
        public const int Address = DeviceName.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="DeviceName"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<string> GetPayload(HarpMessage message)
        {
            return DeviceName.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the unique serial number of the device.
    /// </summary>
    [Description("Specifies the unique serial number of the device.")]
    public partial class SerialNumber
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// SerialNumber register.
    /// </summary>
    /// <seealso cref="SerialNumber"/>
    [Description("Filters and selects timestamped messages from the SerialNumber register.")]
    public partial class TimestampedSerialNumber
    {
        /// <summary>
        /// Represents the address of the <see cref="SerialNumber"/> register. This field is constant.
        /// </summary>
        public const int Address = SerialNumber.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="SerialNumber"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ushort> GetPayload(HarpMessage message)
        {
            return SerialNumber.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents a register that specifies the configuration for the device synchronization clock.
    /// </summary>
    [Description("Specifies the configuration for the device synchronization clock.")]
    public partial class ClockConfiguration
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
    }

    /// <summary>
    /// Provides methods for manipulating timestamped messages from the
    /// ClockConfiguration register.
    /// </summary>
    /// <seealso cref="ClockConfiguration"/>
    [Description("Filters and selects timestamped messages from the ClockConfiguration register.")]
    public partial class TimestampedClockConfiguration
    {
        /// <summary>
        /// Represents the address of the <see cref="ClockConfiguration"/> register. This field is constant.
        /// </summary>
        public const int Address = ClockConfiguration.Address;

        /// <summary>
        /// Returns timestamped payload data for <see cref="ClockConfiguration"/> register messages.
        /// </summary>
        /// <param name="message">A <see cref="HarpMessage"/> object representing the register message.</param>
        /// <returns>A value representing the timestamped message payload.</returns>
        public static Timestamped<ClockConfigurationFlags> GetPayload(HarpMessage message)
        {
            return ClockConfiguration.GetTimestampedPayload(message);
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the identity class of the device.
    /// </summary>
    [DisplayName("WhoAmIPayload")]
    [Description("Creates a message payload that specifies the identity class of the device.")]
    public partial class CreateWhoAmIPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the identity class of the device.
        /// </summary>
        [Description("The value that specifies the identity class of the device.")]
        public int WhoAmI { get; set; }

        /// <summary>
        /// Creates a message payload for the WhoAmI register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public int GetPayload()
        {
            return WhoAmI;
        }

        /// <summary>
        /// Creates a message that specifies the identity class of the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the WhoAmI register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.WhoAmI.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the identity class of the device.
    /// </summary>
    [DisplayName("TimestampedWhoAmIPayload")]
    [Description("Creates a timestamped message payload that specifies the identity class of the device.")]
    public partial class CreateTimestampedWhoAmIPayload : CreateWhoAmIPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the identity class of the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the WhoAmI register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.WhoAmI.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the major hardware version of the device.
    /// </summary>
    [DisplayName("HardwareVersionHighPayload")]
    [Description("Creates a message payload that specifies the major hardware version of the device.")]
    public partial class CreateHardwareVersionHighPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the major hardware version of the device.
        /// </summary>
        [Description("The value that specifies the major hardware version of the device.")]
        public byte HardwareVersionHigh { get; set; }

        /// <summary>
        /// Creates a message payload for the HardwareVersionHigh register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return HardwareVersionHigh;
        }

        /// <summary>
        /// Creates a message that specifies the major hardware version of the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the HardwareVersionHigh register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.HardwareVersionHigh.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the major hardware version of the device.
    /// </summary>
    [DisplayName("TimestampedHardwareVersionHighPayload")]
    [Description("Creates a timestamped message payload that specifies the major hardware version of the device.")]
    public partial class CreateTimestampedHardwareVersionHighPayload : CreateHardwareVersionHighPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the major hardware version of the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the HardwareVersionHigh register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.HardwareVersionHigh.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the minor hardware version of the device.
    /// </summary>
    [DisplayName("HardwareVersionLowPayload")]
    [Description("Creates a message payload that specifies the minor hardware version of the device.")]
    public partial class CreateHardwareVersionLowPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the minor hardware version of the device.
        /// </summary>
        [Description("The value that specifies the minor hardware version of the device.")]
        public byte HardwareVersionLow { get; set; }

        /// <summary>
        /// Creates a message payload for the HardwareVersionLow register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return HardwareVersionLow;
        }

        /// <summary>
        /// Creates a message that specifies the minor hardware version of the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the HardwareVersionLow register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.HardwareVersionLow.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the minor hardware version of the device.
    /// </summary>
    [DisplayName("TimestampedHardwareVersionLowPayload")]
    [Description("Creates a timestamped message payload that specifies the minor hardware version of the device.")]
    public partial class CreateTimestampedHardwareVersionLowPayload : CreateHardwareVersionLowPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the minor hardware version of the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the HardwareVersionLow register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.HardwareVersionLow.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the version of the assembled components in the device.
    /// </summary>
    [DisplayName("AssemblyVersionPayload")]
    [Description("Creates a message payload that specifies the version of the assembled components in the device.")]
    public partial class CreateAssemblyVersionPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the version of the assembled components in the device.
        /// </summary>
        [Description("The value that specifies the version of the assembled components in the device.")]
        public byte AssemblyVersion { get; set; }

        /// <summary>
        /// Creates a message payload for the AssemblyVersion register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return AssemblyVersion;
        }

        /// <summary>
        /// Creates a message that specifies the version of the assembled components in the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the AssemblyVersion register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.AssemblyVersion.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the version of the assembled components in the device.
    /// </summary>
    [DisplayName("TimestampedAssemblyVersionPayload")]
    [Description("Creates a timestamped message payload that specifies the version of the assembled components in the device.")]
    public partial class CreateTimestampedAssemblyVersionPayload : CreateAssemblyVersionPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the version of the assembled components in the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the AssemblyVersion register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.AssemblyVersion.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the major version of the Harp core implemented by the device.
    /// </summary>
    [DisplayName("CoreVersionHighPayload")]
    [Description("Creates a message payload that specifies the major version of the Harp core implemented by the device.")]
    public partial class CreateCoreVersionHighPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the major version of the Harp core implemented by the device.
        /// </summary>
        [Description("The value that specifies the major version of the Harp core implemented by the device.")]
        public byte CoreVersionHigh { get; set; }

        /// <summary>
        /// Creates a message payload for the CoreVersionHigh register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return CoreVersionHigh;
        }

        /// <summary>
        /// Creates a message that specifies the major version of the Harp core implemented by the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the CoreVersionHigh register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.CoreVersionHigh.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the major version of the Harp core implemented by the device.
    /// </summary>
    [DisplayName("TimestampedCoreVersionHighPayload")]
    [Description("Creates a timestamped message payload that specifies the major version of the Harp core implemented by the device.")]
    public partial class CreateTimestampedCoreVersionHighPayload : CreateCoreVersionHighPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the major version of the Harp core implemented by the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the CoreVersionHigh register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.CoreVersionHigh.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the minor version of the Harp core implemented by the device.
    /// </summary>
    [DisplayName("CoreVersionLowPayload")]
    [Description("Creates a message payload that specifies the minor version of the Harp core implemented by the device.")]
    public partial class CreateCoreVersionLowPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the minor version of the Harp core implemented by the device.
        /// </summary>
        [Description("The value that specifies the minor version of the Harp core implemented by the device.")]
        public byte CoreVersionLow { get; set; }

        /// <summary>
        /// Creates a message payload for the CoreVersionLow register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return CoreVersionLow;
        }

        /// <summary>
        /// Creates a message that specifies the minor version of the Harp core implemented by the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the CoreVersionLow register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.CoreVersionLow.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the minor version of the Harp core implemented by the device.
    /// </summary>
    [DisplayName("TimestampedCoreVersionLowPayload")]
    [Description("Creates a timestamped message payload that specifies the minor version of the Harp core implemented by the device.")]
    public partial class CreateTimestampedCoreVersionLowPayload : CreateCoreVersionLowPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the minor version of the Harp core implemented by the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the CoreVersionLow register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.CoreVersionLow.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the major version of the Harp core implemented by the device.
    /// </summary>
    [DisplayName("FirmwareVersionHighPayload")]
    [Description("Creates a message payload that specifies the major version of the Harp core implemented by the device.")]
    public partial class CreateFirmwareVersionHighPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the major version of the Harp core implemented by the device.
        /// </summary>
        [Description("The value that specifies the major version of the Harp core implemented by the device.")]
        public byte FirmwareVersionHigh { get; set; }

        /// <summary>
        /// Creates a message payload for the FirmwareVersionHigh register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return FirmwareVersionHigh;
        }

        /// <summary>
        /// Creates a message that specifies the major version of the Harp core implemented by the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FirmwareVersionHigh register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.FirmwareVersionHigh.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the major version of the Harp core implemented by the device.
    /// </summary>
    [DisplayName("TimestampedFirmwareVersionHighPayload")]
    [Description("Creates a timestamped message payload that specifies the major version of the Harp core implemented by the device.")]
    public partial class CreateTimestampedFirmwareVersionHighPayload : CreateFirmwareVersionHighPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the major version of the Harp core implemented by the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FirmwareVersionHigh register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.FirmwareVersionHigh.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the minor version of the Harp core implemented by the device.
    /// </summary>
    [DisplayName("FirmwareVersionLowPayload")]
    [Description("Creates a message payload that specifies the minor version of the Harp core implemented by the device.")]
    public partial class CreateFirmwareVersionLowPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the minor version of the Harp core implemented by the device.
        /// </summary>
        [Description("The value that specifies the minor version of the Harp core implemented by the device.")]
        public byte FirmwareVersionLow { get; set; }

        /// <summary>
        /// Creates a message payload for the FirmwareVersionLow register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public byte GetPayload()
        {
            return FirmwareVersionLow;
        }

        /// <summary>
        /// Creates a message that specifies the minor version of the Harp core implemented by the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the FirmwareVersionLow register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.FirmwareVersionLow.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the minor version of the Harp core implemented by the device.
    /// </summary>
    [DisplayName("TimestampedFirmwareVersionLowPayload")]
    [Description("Creates a timestamped message payload that specifies the minor version of the Harp core implemented by the device.")]
    public partial class CreateTimestampedFirmwareVersionLowPayload : CreateFirmwareVersionLowPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the minor version of the Harp core implemented by the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the FirmwareVersionLow register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.FirmwareVersionLow.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that stores the integral part of the system timestamp, in seconds.
    /// </summary>
    [DisplayName("TimestampSecondsPayload")]
    [Description("Creates a message payload that stores the integral part of the system timestamp, in seconds.")]
    public partial class CreateTimestampSecondsPayload
    {
        /// <summary>
        /// Gets or sets the value that stores the integral part of the system timestamp, in seconds.
        /// </summary>
        [Description("The value that stores the integral part of the system timestamp, in seconds.")]
        public uint TimestampSeconds { get; set; }

        /// <summary>
        /// Creates a message payload for the TimestampSeconds register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public uint GetPayload()
        {
            return TimestampSeconds;
        }

        /// <summary>
        /// Creates a message that stores the integral part of the system timestamp, in seconds.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the TimestampSeconds register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.TimestampSeconds.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that stores the integral part of the system timestamp, in seconds.
    /// </summary>
    [DisplayName("TimestampedTimestampSecondsPayload")]
    [Description("Creates a timestamped message payload that stores the integral part of the system timestamp, in seconds.")]
    public partial class CreateTimestampedTimestampSecondsPayload : CreateTimestampSecondsPayload
    {
        /// <summary>
        /// Creates a timestamped message that stores the integral part of the system timestamp, in seconds.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the TimestampSeconds register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.TimestampSeconds.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that stores the fractional part of the system timestamp, in microseconds.
    /// </summary>
    [DisplayName("TimestampMicrosecondsPayload")]
    [Description("Creates a message payload that stores the fractional part of the system timestamp, in microseconds.")]
    public partial class CreateTimestampMicrosecondsPayload
    {
        /// <summary>
        /// Gets or sets the value that stores the fractional part of the system timestamp, in microseconds.
        /// </summary>
        [Description("The value that stores the fractional part of the system timestamp, in microseconds.")]
        public ushort TimestampMicroseconds { get; set; }

        /// <summary>
        /// Creates a message payload for the TimestampMicroseconds register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return TimestampMicroseconds;
        }

        /// <summary>
        /// Creates a message that stores the fractional part of the system timestamp, in microseconds.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the TimestampMicroseconds register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.TimestampMicroseconds.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that stores the fractional part of the system timestamp, in microseconds.
    /// </summary>
    [DisplayName("TimestampedTimestampMicrosecondsPayload")]
    [Description("Creates a timestamped message payload that stores the fractional part of the system timestamp, in microseconds.")]
    public partial class CreateTimestampedTimestampMicrosecondsPayload : CreateTimestampMicrosecondsPayload
    {
        /// <summary>
        /// Creates a timestamped message that stores the fractional part of the system timestamp, in microseconds.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the TimestampMicroseconds register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.TimestampMicroseconds.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that stores the configuration mode of the device.
    /// </summary>
    [DisplayName("OperationControlPayload")]
    [Description("Creates a message payload that stores the configuration mode of the device.")]
    public partial class CreateOperationControlPayload
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
        /// Creates a message payload for the OperationControl register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public OperationControlPayload GetPayload()
        {
            OperationControlPayload value;
            value.OperationMode = OperationMode;
            value.DumpRegisters = DumpRegisters;
            value.MuteReplies = MuteReplies;
            value.VisualIndicators = VisualIndicators;
            value.OperationLed = OperationLed;
            value.Heartbeat = Heartbeat;
            return value;
        }

        /// <summary>
        /// Creates a message that stores the configuration mode of the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the OperationControl register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.OperationControl.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that stores the configuration mode of the device.
    /// </summary>
    [DisplayName("TimestampedOperationControlPayload")]
    [Description("Creates a timestamped message payload that stores the configuration mode of the device.")]
    public partial class CreateTimestampedOperationControlPayload : CreateOperationControlPayload
    {
        /// <summary>
        /// Creates a timestamped message that stores the configuration mode of the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the OperationControl register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.OperationControl.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that resets the device and saves non-volatile registers.
    /// </summary>
    [DisplayName("ResetDevicePayload")]
    [Description("Creates a message payload that resets the device and saves non-volatile registers.")]
    public partial class CreateResetDevicePayload
    {
        /// <summary>
        /// Gets or sets the value that resets the device and saves non-volatile registers.
        /// </summary>
        [Description("The value that resets the device and saves non-volatile registers.")]
        public ResetFlags ResetDevice { get; set; }

        /// <summary>
        /// Creates a message payload for the ResetDevice register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ResetFlags GetPayload()
        {
            return ResetDevice;
        }

        /// <summary>
        /// Creates a message that resets the device and saves non-volatile registers.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ResetDevice register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.ResetDevice.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that resets the device and saves non-volatile registers.
    /// </summary>
    [DisplayName("TimestampedResetDevicePayload")]
    [Description("Creates a timestamped message payload that resets the device and saves non-volatile registers.")]
    public partial class CreateTimestampedResetDevicePayload : CreateResetDevicePayload
    {
        /// <summary>
        /// Creates a timestamped message that resets the device and saves non-volatile registers.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ResetDevice register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.ResetDevice.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that stores the user-specified device name.
    /// </summary>
    [DisplayName("DeviceNamePayload")]
    [Description("Creates a message payload that stores the user-specified device name.")]
    public partial class CreateDeviceNamePayload
    {
        /// <summary>
        /// Gets or sets the value that stores the user-specified device name.
        /// </summary>
        [Description("The value that stores the user-specified device name.")]
        public string DeviceName { get; set; }

        /// <summary>
        /// Creates a message payload for the DeviceName register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public string GetPayload()
        {
            return DeviceName;
        }

        /// <summary>
        /// Creates a message that stores the user-specified device name.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the DeviceName register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.DeviceName.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that stores the user-specified device name.
    /// </summary>
    [DisplayName("TimestampedDeviceNamePayload")]
    [Description("Creates a timestamped message payload that stores the user-specified device name.")]
    public partial class CreateTimestampedDeviceNamePayload : CreateDeviceNamePayload
    {
        /// <summary>
        /// Creates a timestamped message that stores the user-specified device name.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the DeviceName register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.DeviceName.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the unique serial number of the device.
    /// </summary>
    [DisplayName("SerialNumberPayload")]
    [Description("Creates a message payload that specifies the unique serial number of the device.")]
    public partial class CreateSerialNumberPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the unique serial number of the device.
        /// </summary>
        [Description("The value that specifies the unique serial number of the device.")]
        public ushort SerialNumber { get; set; }

        /// <summary>
        /// Creates a message payload for the SerialNumber register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ushort GetPayload()
        {
            return SerialNumber;
        }

        /// <summary>
        /// Creates a message that specifies the unique serial number of the device.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the SerialNumber register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.SerialNumber.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the unique serial number of the device.
    /// </summary>
    [DisplayName("TimestampedSerialNumberPayload")]
    [Description("Creates a timestamped message payload that specifies the unique serial number of the device.")]
    public partial class CreateTimestampedSerialNumberPayload : CreateSerialNumberPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the unique serial number of the device.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the SerialNumber register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.SerialNumber.FromPayload(timestamp, messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a message payload
    /// that specifies the configuration for the device synchronization clock.
    /// </summary>
    [DisplayName("ClockConfigurationPayload")]
    [Description("Creates a message payload that specifies the configuration for the device synchronization clock.")]
    public partial class CreateClockConfigurationPayload
    {
        /// <summary>
        /// Gets or sets the value that specifies the configuration for the device synchronization clock.
        /// </summary>
        [Description("The value that specifies the configuration for the device synchronization clock.")]
        public ClockConfigurationFlags ClockConfiguration { get; set; }

        /// <summary>
        /// Creates a message payload for the ClockConfiguration register.
        /// </summary>
        /// <returns>The created message payload value.</returns>
        public ClockConfigurationFlags GetPayload()
        {
            return ClockConfiguration;
        }

        /// <summary>
        /// Creates a message that specifies the configuration for the device synchronization clock.
        /// </summary>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new message for the ClockConfiguration register.</returns>
        public HarpMessage GetMessage(MessageType messageType)
        {
            return Bonsai.Harp.ClockConfiguration.FromPayload(messageType, GetPayload());
        }
    }

    /// <summary>
    /// Represents an operator that creates a timestamped message payload
    /// that specifies the configuration for the device synchronization clock.
    /// </summary>
    [DisplayName("TimestampedClockConfigurationPayload")]
    [Description("Creates a timestamped message payload that specifies the configuration for the device synchronization clock.")]
    public partial class CreateTimestampedClockConfigurationPayload : CreateClockConfigurationPayload
    {
        /// <summary>
        /// Creates a timestamped message that specifies the configuration for the device synchronization clock.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message payload, in seconds.</param>
        /// <param name="messageType">Specifies the type of the created message.</param>
        /// <returns>A new timestamped message for the ClockConfiguration register.</returns>
        public HarpMessage GetMessage(double timestamp, MessageType messageType)
        {
            return Bonsai.Harp.ClockConfiguration.FromPayload(timestamp, messageType, GetPayload());
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
