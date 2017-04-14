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
    public enum WearCommandType : byte
    {
        StartAcquisition,
        StopAcquisition,
        StartStimulation,

        WritePositionMotor0,
        WritePositionMotor1,

        WriteOutput0,
        WriteOutput1
    }

    public class Wear : SelectBuilder, INamedElement
    {
        public Wear()
        {
            Type = WearCommandType.StartAcquisition;
        }

        string INamedElement.Name
        {
            get { return typeof(Wear).Name + "." + Type.ToString(); }
        }

        public WearCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case WearCommandType.StartAcquisition:
                    return Expression.Call(typeof(Wear), "ProcessStartAcquisition", new[] { expression.Type }, expression);
                case WearCommandType.StopAcquisition:
                    return Expression.Call(typeof(Wear), "ProcessStopAcquisition", new[] { expression.Type }, expression);
                case WearCommandType.StartStimulation:
                    return Expression.Call(typeof(Wear), "ProcessStartStimulation", new[] { expression.Type }, expression);


                case WearCommandType.WritePositionMotor0:
                    if (expression.Type != typeof(UInt16))
                    {
                        expression = Expression.Convert(expression, typeof(UInt16));
                    }
                    return Expression.Call(typeof(Wear), "ProcessWritePositionMotor0", null, expression);
                case WearCommandType.WritePositionMotor1:
                    if (expression.Type != typeof(UInt16))
                    {
                        expression = Expression.Convert(expression, typeof(UInt16));
                    }
                    return Expression.Call(typeof(Wear), "ProcessWritePositionMotor1", null, expression);

                case WearCommandType.WriteOutput0:
                    if (expression.Type != typeof(bool))
                    {
                        expression = Expression.Convert(expression, typeof(bool));
                    }
                    return Expression.Call(typeof(Wear), "ProcessWriteOutput0", null, expression);
                case WearCommandType.WriteOutput1:
                    if (expression.Type != typeof(bool))
                    {
                        expression = Expression.Convert(expression, typeof(bool));
                    }
                    return Expression.Call(typeof(Wear), "ProcessWriteOutput1", null, expression);

                default:
                    break;
            }
            return expression;
        }

        static HarpDataFrame ProcessStartAcquisition<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 32, 255, (byte)HarpType.U8, 1, 0));
        }

        static HarpDataFrame ProcessStopAcquisition<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 32, 255, (byte)HarpType.U8, 0, 0));
        }

        static HarpDataFrame ProcessStartStimulation<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 33, 255, (byte)HarpType.U8, 1, 0));
        }


        static HarpDataFrame ProcessWritePositionMotor0(UInt16 input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 6, 80, 255, (byte)HarpType.U16, (byte)(input & 255), (byte)((input >> 8) & 255), 0));
        }

        static HarpDataFrame ProcessWritePositionMotor1(UInt16 input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 6, 85, 255, (byte)HarpType.U16, (byte)(input & 255), (byte)((input >> 8) & 255), 0));
        }


        static HarpDataFrame ProcessWriteOutput0(bool input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 38, 255, (byte)HarpType.U8, (byte)(input ? 1 : 0), 0));
        }

        static HarpDataFrame ProcessWriteOutput1(bool input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 39, 255, (byte)HarpType.U8, (byte)(input ? 1 : 0), 0));
        }
    }
}
