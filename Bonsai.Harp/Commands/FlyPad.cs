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
    public enum FlyPadCommandType : byte
    {
        StartAcquisition,
        StopAcquisition,

        DigitalOut,

        GainCh1,
        GainCh2,

        EnableAverage,
        DisableAverage,

        AverageTimeConstant
    }

    [Description(
        "\n" +
        "StartAcquisition: Any\n" +
        "StopAcquisition: Any\n" +
        "\n" +
        "DigitalOut: Boolean\n" +
        "\n" +
        "GainCh1: Positive Integer\n" +
        "GainCh2: Positive Integer\n" +
        "\n" +
        "EnableAverage: Any\n" +
        "DisableAverage: Any\n" +
        "\n" +
        "AverageTimeConstant: Integer\n"

    )]

    public class FlyPad : SelectBuilder, INamedElement
    {
        public FlyPad()
        {
            Type = FlyPadCommandType.StartAcquisition;
        }

        string INamedElement.Name
        {
            get { return typeof(FlyPad).Name + "." + Type.ToString(); }
        }

        public FlyPadCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                /************************************************************************/
                /* Register: START_ACQ                                                 */
                /************************************************************************/
                case FlyPadCommandType.StartAcquisition:
                    return Expression.Call(typeof(FlyPad), "ProcessStartAcquisition", new[] { expression.Type }, expression);
                case FlyPadCommandType.StopAcquisition:
                    return Expression.Call(typeof(FlyPad), "ProcessStopAcquisition", new[] { expression.Type }, expression);

                /************************************************************************/
                /* Registers: DIGITAL_OUT                                               */
                /************************************************************************/
                case FlyPadCommandType.DigitalOut:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(FlyPad), "ProcessDigitalOut", null, expression);

                /************************************************************************/
                /* Register: USE_AVERAGE                                                */
                /************************************************************************/
                case FlyPadCommandType.EnableAverage:
                    return Expression.Call(typeof(FlyPad), "ProcessEnableAverage", new[] { expression.Type }, expression);
                case FlyPadCommandType.DisableAverage:
                    return Expression.Call(typeof(FlyPad), "ProcessDisableAverage", new[] { expression.Type }, expression);

                /************************************************************************/
                /* Registers: GAIN_CHANNELS_1, GAIN_CHANNELS_2                          */
                /************************************************************************/
                case FlyPadCommandType.GainCh1:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(byte)); }
                    return Expression.Call(typeof(FlyPad), "ProcessGainCh1", null, expression);
                case FlyPadCommandType.GainCh2:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(byte)); }
                    return Expression.Call(typeof(FlyPad), "ProcessGainCh2", null, expression);

                /************************************************************************/
                /* Registers: AVERAGE_TIME_CONSTANT                                     */
                /************************************************************************/
                case FlyPadCommandType.AverageTimeConstant:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(byte)); }
                    return Expression.Call(typeof(FlyPad), "ProcessAverageTimeConstant", null, expression);

                default:
                    break;
            }
            return expression;
        }

        /************************************************************************/
        /* Register: START_ACQ                                                  */
        /************************************************************************/
        static HarpDataFrame ProcessStartAcquisition<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 32, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessStopAcquisition<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 32, 255, (byte)HarpType.U8, 0, 0));
        }

        /************************************************************************/
        /* Register: DIGITAL_OUT                                                */
        /************************************************************************/
        static HarpDataFrame ProcessDigitalOut(bool input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 35, 255, (byte)HarpType.U8, input ? (byte)1 : (byte)0, 0));
        }

        /************************************************************************/
        /* Register: USE_AVERAGE                                                  */
        /************************************************************************/
        static HarpDataFrame ProcessEnableAverage<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 41, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessDisableAverage<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 41, 255, (byte)HarpType.U8, 0, 0));
        }

        /************************************************************************/
        /* Registers: GAIN_CHANNELS_1, GAIN_CHANNELS_2                          */
        /************************************************************************/
        static HarpDataFrame ProcessGainCh1(byte input)
        {
            if (input > 3) input = 3;
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 39, 255, (byte)HarpType.U8, input, 0));
        }
        static HarpDataFrame ProcessGainCh2(byte input)
        {
            if (input > 3) input = 3;
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 40, 255, (byte)HarpType.U8, input, 0));
        }

        /************************************************************************/
        /* Registers: AVERAGE_TIME_CONSTANT                                     */
        /************************************************************************/
        static HarpDataFrame ProcessAverageTimeConstant(byte input)
        {
            if (input > 15) input = 15;
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 42, 255, (byte)HarpType.U8, input, 0));
        }
    }
}
