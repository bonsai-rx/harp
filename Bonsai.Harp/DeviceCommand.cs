using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.ComponentModel;

/* Commands should try to convert the input to the used input type.         */
/*                                                                          */
/* Words like Set, Clear, Start, Stop, Enable, Disable or Write should be:  */
/*   - a prefix if the Command doesn't depend on the input (input is any).  */
/*   - a suffix if the Command doesn't depend on the input (input is any).  */
/*   - avoided when the Command depend on the input                         */

/* Example of Commands' descriptions            */
/*      Any                                     */
/*      Boolean                                 */
/*      Bitmask                                 */
/*      Integer                                 */
/*      Positive integer                        */
/*      Decimal                                 */
/*      Positive decimal                        */
/*      Integer array[9]                        */
/*      Positive integer array[9]               */

namespace Bonsai.Harp
{
    public enum DeviceCommandType : byte
    {
        WriteTimestamp,
        SynchronizeTimestamp
    }

    [Description(
    "\n" +
    "Timestamp: Positive integer\n" +
    "SynchronizeTimestamp: Any\n"
    )]

    public class DeviceCommand : SelectBuilder, INamedElement
    {
        public DeviceCommand()
        {
            Type = DeviceCommandType.SynchronizeTimestamp;
        }

        string INamedElement.Name
        {
            get { return typeof(DeviceCommand).Name.Replace("Command", string.Empty) + "." + Type.ToString(); }
        }

        public DeviceCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                /************************************************************************/
                /* Register: R_TIMESTAMP_SECOND                                         */
                /************************************************************************/
                case DeviceCommandType.WriteTimestamp:
                    if (expression.Type != typeof(UInt32))
                    {
                        expression = Expression.Convert(expression, typeof(UInt32));
                    }
                    return Expression.Call(typeof(DeviceCommand), "ProcessWriteTimestamp", null, expression);


                case DeviceCommandType.SynchronizeTimestamp:
                    return Expression.Call(typeof(DeviceCommand), "ProcessSynchronizeTimestamp", new[] { expression.Type }, expression);

                default:
                    break;
            }
            return expression;
        }

        /************************************************************************/
        /* Register: R_TIMESTAMP_SECOND                                         */
        /************************************************************************/
        static HarpMessage ProcessWriteTimestamp(UInt32 input)
        {
            return new HarpMessage(true, 2, 8, 8, 255, (byte)PayloadType.U32, (byte)(input & 255), (byte)((input >> 8) & 255), (byte)((input >> 16) & 255), (byte)((input >> 24) & 255), 0);
        }

        static HarpMessage ProcessSynchronizeTimestamp<TSource>(TSource input)
        {
            UInt32 unixTimestamp = (UInt32)(DateTime.UtcNow.Subtract(new DateTime(1904, 1, 1))).TotalSeconds;

            return new HarpMessage(true, 2, 8, 8, 255, (byte)PayloadType.U32, (byte)(unixTimestamp & 255), (byte)((unixTimestamp >> 8) & 255), (byte)((unixTimestamp >> 16) & 255), (byte)((unixTimestamp >> 24) & 255), 0);
        }
    }
}
