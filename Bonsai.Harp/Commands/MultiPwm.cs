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

    public enum MultiPwmCommandType : byte
    {
        SetTrigger0 = 0,
        SetTrigger1,
        SetTrigger2,
        SetTrigger3,
        TriggersSet,

        ClearTrigger0,
        ClearTrigger1,
        ClearTrigger2,
        ClearTrigger3,
        TriggersClear,

        OutputsEnable
    }

    [Description(
        "\n" +
        "SetTrigger0: Any\n" +
        "SetTrigger1: Any\n" +
        "SetTrigger2: Any\n" +
        "SetTrigger3: Any\n" +
        "TriggersSet: Bitmask\n" +      // Don't need to indicate it's a byte since the code makes the conversion
        "\n" +
        "ClearTrigger0: Any\n" +
        "ClearTrigger1: Any\n" +
        "ClearTrigger2: Any\n" +
        "ClearTrigger3: Any\n" +
        "TriggersClear: Bitmask\n" +    // Don't need to indicate it's a byte since the code makes the conversion
        "\n" +
        "OutputsEnable: Bitmask\n"
    )]

    public class MultiPwm : SelectBuilder, INamedElement
    {
        public MultiPwm()
        {
            Type = MultiPwmCommandType.SetTrigger0;
        }

        string INamedElement.Name
        {
            get { return typeof(MultiPwm).Name + "." + Type.ToString(); }
        }

        public MultiPwmCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                /************************************************************************/
                /* Register: START_PWM                                                  */
                /************************************************************************/
                case MultiPwmCommandType.SetTrigger0:
                    return Expression.Call(typeof(MultiPwm), "ProcessSetTrigger0", new[] { expression.Type }, expression);
                case MultiPwmCommandType.SetTrigger1:
                    return Expression.Call(typeof(MultiPwm), "ProcessSetTrigger1", new[] { expression.Type }, expression);
                case MultiPwmCommandType.SetTrigger2:
                    return Expression.Call(typeof(MultiPwm), "ProcessSetTrigger2", new[] { expression.Type }, expression);
                case MultiPwmCommandType.SetTrigger3:
                    return Expression.Call(typeof(MultiPwm), "ProcessSetTrigger3", new[] { expression.Type }, expression);

                case MultiPwmCommandType.TriggersSet:
                    if (expression.Type != typeof(byte)) { expression = Expression.Convert(expression, typeof(byte)); }     // If possible, convert to byte
                    return Expression.Call(typeof(MultiPwm), "ProcessTriggersSet", null, expression);

                /************************************************************************/
                /* Register: STOP_PWM                                                   */
                /************************************************************************/
                case MultiPwmCommandType.ClearTrigger0:
                    return Expression.Call(typeof(MultiPwm), "ProcessClearTrigger0", new[] { expression.Type }, expression);
                case MultiPwmCommandType.ClearTrigger1:
                    return Expression.Call(typeof(MultiPwm), "ProcessClearTrigger1", new[] { expression.Type }, expression);
                case MultiPwmCommandType.ClearTrigger2:
                    return Expression.Call(typeof(MultiPwm), "ProcessClearTrigger2", new[] { expression.Type }, expression);
                case MultiPwmCommandType.ClearTrigger3:
                    return Expression.Call(typeof(MultiPwm), "ProcessClearTrigger3", new[] { expression.Type }, expression);

                case MultiPwmCommandType.TriggersClear:
                    if (expression.Type != typeof(byte)) { expression = Expression.Convert(expression, typeof(byte)); }     // If possible, convert to byte
                    return Expression.Call(typeof(MultiPwm), "ProcessTriggersClear", null, expression);

                /************************************************************************/
                /* Register: CH_ENABLE                                                  */
                /************************************************************************/
                case MultiPwmCommandType.OutputsEnable:
                    if (expression.Type != typeof(byte)) { expression = Expression.Convert(expression, typeof(byte)); }     // If possible, convert to byte
                    return Expression.Call(typeof(MultiPwm), "ProcessOutputsEnable", null, expression);

                default:
                    break;
            }
            return expression;
        }


        /************************************************************************/
        /* Register: START_PWM                                                  */
        /************************************************************************/
        static HarpDataFrame ProcessSetTrigger0<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 60, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessSetTrigger1<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 60, 255, (byte)HarpType.U8, 2, 0));
        }
        static HarpDataFrame ProcessSetTrigger2<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 60, 255, (byte)HarpType.U8, 4, 0));
        }
        static HarpDataFrame ProcessSetTrigger3<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 60, 255, (byte)HarpType.U8, 8, 0));
        }
        static HarpDataFrame ProcessTriggersSet(byte input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 60, 255, (byte)HarpType.U8, (byte)(input & 0x0F), 0));
        }

        /************************************************************************/
        /* Register: STOP_PWM                                                   */
        /************************************************************************/
        static HarpDataFrame ProcessClearTrigger0<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 61, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessClearTrigger1<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 61, 255, (byte)HarpType.U8, 2, 0));
        }
        static HarpDataFrame ProcessClearTrigger2<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 61, 255, (byte)HarpType.U8, 4, 0));
        }
        static HarpDataFrame ProcessClearTrigger3<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 61, 255, (byte)HarpType.U8, 8, 0));
        }
        static HarpDataFrame ProcessTriggersClear(byte input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 61, 255, (byte)HarpType.U8, (byte)(input & 0x0F), 0));
        }

        /************************************************************************/
        /* Register: CH_ENABLE                                                  */
        /************************************************************************/
        static HarpDataFrame ProcessOutputsEnable(byte input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 69, 255, (byte)HarpType.U8, (byte)(input & 0x0F), 0));
        }
    }
}
