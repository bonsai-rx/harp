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
    [Obsolete]
    [XmlInclude(typeof(SetTimestamp))]
    [XmlInclude(typeof(SynchronizeTimestamp))]
    [XmlInclude(typeof(OperationControl))]
    [XmlInclude(typeof(ResetDevice))]
    [WorkflowElementCategory(ElementCategory.Transform)]
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

        /// <summary>
        /// Gets a value indicating whether the <see cref="Type"/> property should be serialized.
        /// </summary>
        [Obsolete]
        [Browsable(false)]
        public bool TypeSpecified
        {
            get { return false; }
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
    [Obsolete]
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
}
