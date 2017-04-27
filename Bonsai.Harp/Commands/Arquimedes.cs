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
    public enum ArquimedesCommandType : byte
    {
        WriteLoadPosition,

        ResetLeverAngle,
        ResetLoadPosition,

        WriteHideLever,

        WriteOutput0,
        WriteOutput1,
        WriteOutput2,
        WriteOutput3,
        WriteOutput4,
        WriteOutput5,

        EnableLedConfiguration0,
        EnableLedConfiguration1,
        EnableLedConfiguration2,
        EnableLedConfiguration3,
        EnableLedConfiguration4,
        EnableLedConfiguration5,
        EnableLedConfiguration6,
        EnableLedConfiguration7,     

        WriteLeds
    }

    public class Arquimedes : SelectBuilder, INamedElement
    {
        public Arquimedes()
        {
            Type = ArquimedesCommandType.WriteLoadPosition;
        }

        string INamedElement.Name
        {
            get { return typeof(Arquimedes).Name + "." + Type.ToString(); }
        }

        public ArquimedesCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case ArquimedesCommandType.WriteLoadPosition:
                    if (expression.Type != typeof(UInt16)) { expression = Expression.Convert(expression, typeof(UInt16)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteLoadPosition", null, expression);

                case ArquimedesCommandType.ResetLeverAngle:
                    return Expression.Call(typeof(Arquimedes), "ProcessResetLeverAngle", new[] { expression.Type }, expression);
                case ArquimedesCommandType.ResetLoadPosition:
                    return Expression.Call(typeof(Arquimedes), "ProcessResetLoadPosition", new[] { expression.Type }, expression);

                case ArquimedesCommandType.WriteHideLever:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteHideLever", null, expression);

                case ArquimedesCommandType.WriteOutput0:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteOutput0", null, expression);
                case ArquimedesCommandType.WriteOutput1:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteOutput1", null, expression);
                case ArquimedesCommandType.WriteOutput2:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteOutput2", null, expression);
                case ArquimedesCommandType.WriteOutput3:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteOutput3", null, expression);
                case ArquimedesCommandType.WriteOutput4:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteOutput4", null, expression);
                case ArquimedesCommandType.WriteOutput5:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteOutput5", null, expression);

                case ArquimedesCommandType.EnableLedConfiguration0:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessEnableLedConfiguration0", null, expression);
                case ArquimedesCommandType.EnableLedConfiguration1:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessEnableLedConfiguration1", null, expression);
                case ArquimedesCommandType.EnableLedConfiguration2:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessEnableLedConfiguration2", null, expression);
                case ArquimedesCommandType.EnableLedConfiguration3:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessEnableLedConfiguration3", null, expression);
                case ArquimedesCommandType.EnableLedConfiguration4:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessEnableLedConfiguration4", null, expression);
                case ArquimedesCommandType.EnableLedConfiguration5:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessEnableLedConfiguration5", null, expression);
                case ArquimedesCommandType.EnableLedConfiguration6:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessEnableLedConfiguration6", null, expression);
                case ArquimedesCommandType.EnableLedConfiguration7:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Arquimedes), "ProcessEnableLedConfiguration7", null, expression);

                case ArquimedesCommandType.WriteLeds:
                    if (expression.Type != typeof(byte[])) { expression = Expression.Convert(expression, typeof(byte[])); }
                    return Expression.Call(typeof(Arquimedes), "ProcessWriteLeds", null, expression);

                default:
                    break;
            }
            return expression;
        }




        static HarpDataFrame ProcessWriteLoadPosition(UInt16 input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 6, 56, 255, (byte)HarpType.U16, (byte)(input & 255), (byte)((input >> 8) & 255), 0));
        }

        static HarpDataFrame ProcessResetLeverAngle<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 39, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessResetLoadPosition<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 40, 255, (byte)HarpType.U8, 1, 0));
        }

        static HarpDataFrame ProcessWriteHideLever(bool input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 41, 255, (byte)HarpType.U8, (byte)(input ? 1 : 0), 0));
        }

        static HarpDataFrame CreateHarpFrameForOutputs (bool toHigh, int outputNumber)
        {
            if (toHigh)
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 42, 255, (byte)HarpType.U8, (byte)(1 << outputNumber), 0));
            else
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 43, 255, (byte)HarpType.U8, (byte)(1 << outputNumber), 0));
        }
        static HarpDataFrame ProcessWriteOutput0(bool input) { return CreateHarpFrameForOutputs(input, 0); }
        static HarpDataFrame ProcessWriteOutput1(bool input) { return CreateHarpFrameForOutputs(input, 1); }
        static HarpDataFrame ProcessWriteOutput2(bool input) { return CreateHarpFrameForOutputs(input, 2); }
        static HarpDataFrame ProcessWriteOutput3(bool input) { return CreateHarpFrameForOutputs(input, 3); }
        static HarpDataFrame ProcessWriteOutput4(bool input) { return CreateHarpFrameForOutputs(input, 4); }
        static HarpDataFrame ProcessWriteOutput5(bool input) { return CreateHarpFrameForOutputs(input, 5); }

        static HarpDataFrame CreateHarpFrameForLeds(bool toEnable, int ledNumber)
        {
            if (toEnable)
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 44, 255, (byte)HarpType.U8, (byte)(1 << ledNumber), 0));
            else
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 45, 255, (byte)HarpType.U8, (byte)(1 << ledNumber), 0));
        }
        static HarpDataFrame ProcessEnableLedConfiguration0(bool input) { return CreateHarpFrameForLeds(input, 0); }
        static HarpDataFrame ProcessEnableLedConfiguration1(bool input) { return CreateHarpFrameForLeds(input, 1); }
        static HarpDataFrame ProcessEnableLedConfiguration2(bool input) { return CreateHarpFrameForLeds(input, 2); }
        static HarpDataFrame ProcessEnableLedConfiguration3(bool input) { return CreateHarpFrameForLeds(input, 3); }
        static HarpDataFrame ProcessEnableLedConfiguration4(bool input) { return CreateHarpFrameForLeds(input, 4); }
        static HarpDataFrame ProcessEnableLedConfiguration5(bool input) { return CreateHarpFrameForLeds(input, 5); }
        static HarpDataFrame ProcessEnableLedConfiguration6(bool input) { return CreateHarpFrameForLeds(input, 6); }
        static HarpDataFrame ProcessEnableLedConfiguration7(bool input) { return CreateHarpFrameForLeds(input, 7); }


        static HarpDataFrame ProcessWriteLeds(byte [] RGBs)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 13, 46, 255, (byte)HarpType.U8, RGBs[0], RGBs[1], RGBs[2], RGBs[3], RGBs[4], RGBs[5], RGBs[6], RGBs[7], RGBs[8], 0));
        }
    }
}
