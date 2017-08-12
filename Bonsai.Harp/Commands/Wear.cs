using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp.Commands
{
    public enum WearCommandType : byte
    {
        StartAcquisition,
        StopAcquisition,
        StartStimulation,

        PositionMotor0,
        PositionMotor1,

        Output0,
        Output1
    }

    [Description(
        "\n" +
        "StartAcquisition: Any\n" +
        "StopAcquisition: Any\n" +
        "StartStimulation: Any\n" +
        "\n" +
        "PositionMotor0: Positive integer\n" +    // Don't need to indicate it's a UInt16 since the code makes the conversion
        "PositionMotor1: Positive integer\n" +    // Don't need to indicate it's a UInt16 since the code makes the conversion
        "\n" +
        "Output0: Boolean\n" +
        "Output1: Boolean\n"
    )]
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

                case WearCommandType.PositionMotor0:
                    if (expression.Type != typeof(UInt16)) { expression = Expression.Convert(expression, typeof(UInt16)); }
                    return Expression.Call(typeof(Wear), "ProcessPositionMotor0", null, expression);
                case WearCommandType.PositionMotor1:
                    if (expression.Type != typeof(UInt16)) { expression = Expression.Convert(expression, typeof(UInt16)); }
                    return Expression.Call(typeof(Wear), "ProcessPositionMotor1", null, expression);

                case WearCommandType.Output0:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Wear), "ProcessOutput0", null, expression);
                case WearCommandType.Output1:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Wear), "ProcessOutput1", null, expression);

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


        static HarpDataFrame ProcessPositionMotor0(UInt16 input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 6, 80, 255, (byte)HarpType.U16, (byte)(input & 255), (byte)((input >> 8) & 255), 0));
        }

        static HarpDataFrame ProcessPositionMotor1(UInt16 input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 6, 85, 255, (byte)HarpType.U16, (byte)(input & 255), (byte)((input >> 8) & 255), 0));
        }


        static HarpDataFrame ProcessOutput0(bool input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 38, 255, (byte)HarpType.U8, (byte)(input ? 1 : 0), 0));
        }

        static HarpDataFrame ProcessOutput1(bool input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 39, 255, (byte)HarpType.U8, (byte)(input ? 1 : 0), 0));
        }
    }
}
