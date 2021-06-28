using Bonsai.Expressions;
using System;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator which creates standard command messages available to all Harp devices.
    /// </summary>
    [Description("Creates standard command messages available to all Harp devices.")]
    [TypeDescriptionProvider(typeof(DeviceTypeDescriptionProvider<DeviceCommand>))]
    public class DeviceCommand : SelectBuilder, INamedElement
    {
        /// <summary>
        /// Gets or sets the type of the device command message to create.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("The type of the device command message to create.")]
        public DeviceCommandType Type { get; set; } = DeviceCommandType.SynchronizeTimestamp;

        string INamedElement.Name => $"{nameof(Device)}.{Type}";

        string Description
        {
            get
            {
                switch (Type)
                {
                    case DeviceCommandType.Timestamp: return "Updates the value of the timestamp register in the Harp device, in whole seconds.";
                    case DeviceCommandType.SynchronizeTimestamp: return "Sets the value of the timestamp register in the Harp device to the UTC timestamp of the host.";
                    default: return null;
                }
            }
        }

        /// <summary>
        /// Returns the expression that specifies how the standard command messages are created.
        /// </summary>
        /// <param name="expression">The input parameter to the selector.</param>
        /// <returns>
        /// The <see cref="Expression"/> that maps the input parameter to the
        /// standard command message.
        /// </returns>
        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case DeviceCommandType.Timestamp:
                    if (expression.Type != typeof(uint))
                    {
                        expression = Expression.Convert(expression, typeof(uint));
                    }
                    return Expression.Call(typeof(DeviceCommand), nameof(WriteTimestamp), null, expression);
                case DeviceCommandType.SynchronizeTimestamp:
                    return Expression.Call(typeof(DeviceCommand), nameof(SynchronizeTimestamp), null);
                default:
                    throw new InvalidOperationException("Invalid selection or not supported yet.");
            }
        }

        static HarpMessage WriteTimestamp(uint input)
        {
            return HarpCommand.WriteUInt32(DeviceRegisters.TimestampSecond, input);
        }

        static HarpMessage SynchronizeTimestamp()
        {
            var unixTimestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1904, 1, 1))).TotalSeconds;
            return HarpCommand.WriteUInt32(DeviceRegisters.TimestampSecond, unixTimestamp);
        }
    }

    /// <summary>
    /// Specifies standard device commands available on all Harp devices.
    /// </summary>
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
}
