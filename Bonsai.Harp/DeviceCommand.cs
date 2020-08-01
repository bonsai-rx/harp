using Bonsai.Expressions;
using System;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Bonsai.Harp
{
    [Description("Creates command messages available to all Harp devices.")]
    [TypeDescriptionProvider(typeof(DeviceTypeDescriptionProvider<DeviceCommand>))]
    public class DeviceCommand : SelectBuilder, INamedElement
    {
        [RefreshProperties(RefreshProperties.All)]
        public DeviceCommandType Type { get; set; } = DeviceCommandType.SynchronizeTimestamp;

        string INamedElement.Name => $"Device.{Type}";

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
            return HarpMessage.FromUInt32(Registers.TimestampSecond, MessageType.Write, input);
        }

        static HarpMessage SynchronizeTimestamp()
        {
            var unixTimestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1904, 1, 1))).TotalSeconds;
            return HarpMessage.FromUInt32(Registers.TimestampSecond, MessageType.Write, unixTimestamp);
        }
    }

    public enum DeviceCommandType : byte
    {
        Timestamp,
        SynchronizeTimestamp
    }
}
