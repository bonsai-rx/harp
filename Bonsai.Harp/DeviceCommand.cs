using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which creates standard command messages available to all Harp devices.
    /// </summary>
    /// <seealso cref="SetTimestamp"/>
    /// <seealso cref="SynchronizeTimestamp"/>
    /// <seealso cref="OperationControl"/>
    /// <seealso cref="ResetDevice"/>
    [XmlInclude(typeof(SetTimestamp))]
    [XmlInclude(typeof(SynchronizeTimestamp))]
    [XmlInclude(typeof(OperationControl))]
    [XmlInclude(typeof(ResetDevice))]
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
    public class SetTimestamp : Combinator<uint, HarpMessage>
    {
        /// <summary>
        /// Creates an observable sequence of command messages to set the value of the
        /// timestamp register in the Harp device.
        /// </summary>
        /// <param name="source">
        /// The sequence of timestamp values, in whole seconds, used to reset the Harp
        /// clock register.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing the command
        /// to set the value of the timestamp register in the Harp device.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<uint> source)
        {
            return source.Select(value => HarpCommand.WriteUInt32(DeviceRegisters.TimestampSecond, value));
        }
    }

    /// <summary>
    /// Represents an operator that creates a command message to set the value of
    /// the timestamp register in the Harp device to the current UTC time of the host.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Creates a command message to set the value of the timestamp register in the Harp device to the current UTC time of the host.")]
    public class SynchronizeTimestamp : Combinator<HarpMessage>
    {
        /// <summary>
        /// Creates an observable sequence of command messages to set the value of
        /// the timestamp register in the Harp device to the current UTC time of
        /// the host.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for creating new command
        /// messages.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing the command
        /// to set the value of the timestamp register in the Harp device.
        /// </returns>
        public override IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ =>
            {
                var unixTimestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1904, 1, 1))).TotalSeconds;
                return HarpCommand.WriteUInt32(DeviceRegisters.TimestampSecond, unixTimestamp);
            });
        }
    }

    /// <summary>
    /// Represents an operator that creates a command message to initialize the
    /// operation control register in a Harp device.
    /// </summary>
    [DesignTimeVisible(false)]
    [Description("Creates a command message to initialize the operation control register in a Harp device.")]
    public class OperationControl : Combinator<HarpMessage>
    {
        /// <summary>
        /// Gets or sets a value specifying the desired operation mode of the device.
        /// </summary>
        [Description("Specifies the desired operation mode of the device.")]
        public DeviceState OperationMode { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether the device should report the state
        /// of all registers following initialization.
        /// </summary>
        [Description("Specifies whether the device should report the state of all registers following initialization.")]
        public bool DumpRegisters { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether the operation mode LED should
        /// report the device state.
        /// </summary>
        [Description("Specifies whether the operation mode LED should report the device state.")]
        public LedState LedState { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether the visual indicator LEDs in the
        /// device should be enabled.
        /// </summary>
        [Description("Specifies whether the visual indicator LEDs in the Harp device should be enabled.")]
        public LedState VisualIndicators { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether the device should report the
        /// current timestamp every second.
        /// </summary>
        [Description("Specifies whether the device should report the current timestamp every second.")]
        public EnableType Heartbeat { get; set; }

        /// <summary>
        /// Creates an observable sequence of command messages to initialize the
        /// operation control register in a Harp device.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for creating new command
        /// messages.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing the command
        /// to initialize the operation control register in a Harp device.
        /// </returns>
        public override IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => HarpCommand.OperationControl(
                OperationMode,
                LedState,
                VisualIndicators,
                Heartbeat,
                replies: EnableType.Enable,
                DumpRegisters));
        }
    }

    /// <summary>
    /// Represents an operator that creates a command message to reset the device
    /// and save non-volatile registers.
    /// </summary>
    [Description("Creates a command message to reset the device and save non-volatile registers.")]
    public class ResetDevice : Combinator<HarpMessage>
    {
        /// <summary>
        /// Gets or sets a value specifying the the behavior of the non-volatile
        /// registers when resetting the device.
        /// </summary>
        [Description("Specifies the behavior of the non-volatile registers when resetting the device.")]
        public ResetMode Mode { get; set; }

        /// <summary>
        /// Creates an observable sequence of command messages to reset the device
        /// and save non-volatile registers.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for creating new command
        /// messages.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="HarpMessage"/> objects representing the command
        /// to reset the device and save non-volatile registers.
        /// </returns>
        public override IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => HarpCommand.ResetDevice(Mode));
        }
    }
}
