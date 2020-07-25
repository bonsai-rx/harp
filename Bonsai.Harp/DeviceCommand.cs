using Bonsai.Expressions;
using System;
using System.Linq.Expressions;
using System.ComponentModel;

/* Commands should try to convert the input to the used input type.
 * The input types should be Bonsai typical types like int, boolean and Float.
 * 
 * Use suffix like Set, Clear, Start, Stop, Enable, Disable or don't use any.
 * Suffix like Write should be avoided.
 * 
 * Examples for Commands' input type:
 *      Any
 *      Boolean
 *      Bitmask
 *      Integer
 *      Positive integer
 *      Decimal
 *      Positive decimal
 *      Integer array[9]
 *      Positive integer array[9]
 */

namespace Bonsai.Harp
{
    [Description(
    "\n" +
    "Timestamp: Positive integer\n" +
    "SynchronizeTimestamp: Any\n"
    )]
    public class DeviceCommand : SelectBuilder, INamedElement
    {
        string INamedElement.Name => $"Device.{Type}";

        public DeviceCommandType Type { get; set; } = DeviceCommandType.SynchronizeTimestamp;

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
