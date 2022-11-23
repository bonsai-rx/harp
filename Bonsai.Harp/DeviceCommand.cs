using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which creates standard command messages available to all Harp devices.
    /// </summary>
    [XmlInclude(typeof(SetTimestamp))]
    [XmlInclude(typeof(SynchronizeTimestamp))]
    [Description("Creates standard command messages available to all Harp devices.")]
    public class DeviceCommand : CommandBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCommand"/> class.
        /// </summary>
        public DeviceCommand()
        {
            Command = new SetTimestamp();
        }

        /// <summary>
        /// Gets or sets the type of the device command message to create.
        /// This property is obsolete.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0612 // Type or member is obsolete
        public DeviceCommandType Type
        {
            get
            {
                return Command?.GetType() switch
                {
                    Type type when type == typeof(SetTimestamp) => DeviceCommandType.Timestamp,
                    Type type when type == typeof(SynchronizeTimestamp) => DeviceCommandType.SynchronizeTimestamp,
                    _ => default,
                };
            }
            set
            {
                Command = value switch
                {
                    DeviceCommandType.SynchronizeTimestamp => new SynchronizeTimestamp(),
                    _ => new SetTimestamp(),
                };
            }
        }
#pragma warning restore CS0612 // Type or member is obsolete
    }

    /// <summary>
    /// Specifies standard device commands available on all Harp devices.
    /// </summary>
    [Obsolete]
    public enum DeviceCommandType : byte
    {
        /// <summary>
        /// Specifies that the value of the timestamp register in the Harp device should be updated.
        /// </summary>
        Timestamp,

        /// <summary>
        /// Specifies that the timestamp register in the Harp device should be set to the UTC timestamp of the host.
        /// </summary>
        SynchronizeTimestamp
    }

    /// <summary>
    /// Represents an operator that creates a command message to set the value of
    /// the timestamp register in the Harp device, in whole seconds.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Creates a command message to set the value of the timestamp register in the Harp device, in whole seconds.")]
    public class SetTimestamp : CommandFormatter<uint>
    {
        /// <summary>
        /// Creates a command message to set the value of the timestamp register
        /// in the Harp device, in whole seconds.
        /// </summary>
        /// <param name="value">The value of the timestamp, in whole seconds.</param>
        /// <returns>
        /// A <see cref="HarpMessage"/> object representing the command to set
        /// the timestamp register in the Harp device.
        /// </returns>
        protected override HarpMessage Format(uint value)
        {
            return HarpCommand.WriteUInt32(DeviceRegisters.TimestampSecond, value);
        }
    }

    /// <summary>
    /// Represents an operator that creates a command message to set the value of
    /// the timestamp register in the Harp device to the current UTC time of the host.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Creates a command message to set the value of the timestamp register in the Harp device to the current UTC time of the host.")]
    public class SynchronizeTimestamp : CommandFormatter
    {
        /// <summary>
        /// Creates a command message to set the value of the timestamp register
        /// in the Harp device to the current UTC time of the host.
        /// </summary>
        /// <returns>
        /// A <see cref="HarpMessage"/> object representing the command to synchronize
        /// the timestamp register in the Harp device.
        /// </returns>
        protected override HarpMessage Format()
        {
            var unixTimestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1904, 1, 1))).TotalSeconds;
            return HarpCommand.WriteUInt32(DeviceRegisters.TimestampSecond, unixTimestamp);
        }
    }
}
