using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Bonsai.Harp.Commands
{
    public enum DeviceCommandType : byte
    {
        WriteTimestamp,
        UpdateTimestamp
    }

    public class Device : SelectBuilder, INamedElement
    {
        public Device()
        {
            Type = DeviceCommandType.UpdateTimestamp;
        }

        string INamedElement.Name
        {
            get { return typeof(Device).Name + "." + Type.ToString(); }
        }

        public DeviceCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case DeviceCommandType.WriteTimestamp:
                    if (expression.Type != typeof(UInt32))
                    {
                        expression = Expression.Convert(expression, typeof(UInt32));
                    }
                    return Expression.Call(typeof(Device), "ProcessWriteTimestamp", null, expression);


                case DeviceCommandType.UpdateTimestamp:
                    return Expression.Call(typeof(Device), "ProcessUpdateTimestamp", new[] { expression.Type }, expression);

                default:
                    break;
            }
            return expression;
        }

       static HarpDataFrame ProcessWriteTimestamp(UInt32 input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 8, 8, 255, (byte)HarpType.U32, (byte)(input & 255), (byte)((input >> 8) & 255), (byte)((input >> 16) & 255), (byte)((input >> 24) & 255), 0));
        }

        static HarpDataFrame ProcessUpdateTimestamp<TSource>(TSource input)
        {
            UInt32 unixTimestamp = (UInt32)(DateTime.UtcNow.Subtract(new DateTime(1904, 1, 1))).TotalSeconds;

            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 8, 8, 255, (byte)HarpType.U32, (byte)(unixTimestamp & 255), (byte)((unixTimestamp >> 8) & 255), (byte)((unixTimestamp >> 16) & 255), (byte)((unixTimestamp >> 24) & 255), 0));
        }
    }
}
