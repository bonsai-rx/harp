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
    public enum CameraCommandType : byte
    {
        StartCamera0,
        StartCamera1,
        StartBothCameras,
        StopCamera0,
        StopCamera1,
        StopBothCameras,

        EnableMotor0,
        EnableMotor1,
        EnableBothMotors,
        DisableMotor0,
        DisableMotor1,
        DisableBothMotors,

        PositionMotor0,
        PositionMotor1,

        OutputTrig0,
        OutputSync0,
        OutputTrig1,
        OutputSync1,
        Outputs
    }

    [Description(
        "\n" +
        "StartCamera0: Any\n" +
        "StartCamera1: Any\n" +
        "StartBothCameras: Any\n" +
        "StopCamera0: Any\n" +
        "StopCamera1: Any\n" +
        "StopBothCameras: Any\n" +
        "\n" +
        "EnableMotor0: Any\n" +
        "EnableMotor1: Any\n" +
        "EnableBothMotors: Any\n" +
        "DisableMotor0: Any\n" +
        "DisableMotor1: Any\n" +
        "DisableBothMotors: Any\n" +
        "\n" +
        "PositionMotor0: Positive integer\n" +    // Don't need to indicate it's a UInt16 since the code makes the conversion
        "PositionMotor1: Positive integer\n" +    // Don't need to indicate it's a UInt16 since the code makes the conversion
        "\n" +
        "OutputTrig0: Boolean\n" +
        "OutputTrig1: Boolean\n" +
        "OutputSync0: Boolean\n" +
        "OutputSync1: Boolean\n" +
        "Outputs: Bitmask\n"
    )]
    public class Camera : SelectBuilder, INamedElement
    {
        public Camera()
        {
            Type = CameraCommandType.StartCamera0;
        }

        string INamedElement.Name
        {
            get { return typeof(Camera).Name + "." + Type.ToString(); }
        }

        public CameraCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                /************************************************************************/
                /* Registers: START_CAMS and STOP_CAMS                                  */
                /************************************************************************/
                case CameraCommandType.StartCamera0:
                    return Expression.Call(typeof(Camera), "ProcessStartCamera0", new[] { expression.Type }, expression);
                case CameraCommandType.StartCamera1:
                    return Expression.Call(typeof(Camera), "ProcessStartCamera1", new[] { expression.Type }, expression);
                case CameraCommandType.StartBothCameras:
                    return Expression.Call(typeof(Camera), "ProcessStartBothCameras", new[] { expression.Type }, expression);

                case CameraCommandType.StopCamera0:
                    return Expression.Call(typeof(Camera), "ProcessStopCamera0", new[] { expression.Type }, expression);
                case CameraCommandType.StopCamera1:
                    return Expression.Call(typeof(Camera), "ProcessStopCamera1", new[] { expression.Type }, expression);
                case CameraCommandType.StopBothCameras:
                    return Expression.Call(typeof(Camera), "ProcessStopBothCameras", new[] { expression.Type }, expression);

                /************************************************************************/
                /* Registers: ENABLE_MOTORS and DISABLE_MOTORS                          */
                /************************************************************************/
                case CameraCommandType.EnableMotor0:
                    return Expression.Call(typeof(Camera), "ProcessEnableMotor0", new[] { expression.Type }, expression);
                case CameraCommandType.EnableMotor1:
                    return Expression.Call(typeof(Camera), "ProcessEnableMotor1", new[] { expression.Type }, expression);
                case CameraCommandType.EnableBothMotors:
                    return Expression.Call(typeof(Camera), "ProcessEnableBothMotors", new[] { expression.Type }, expression);

                case CameraCommandType.DisableMotor0:
                    return Expression.Call(typeof(Camera), "ProcessDisableMotor0", new[] { expression.Type }, expression);
                case CameraCommandType.DisableMotor1:
                    return Expression.Call(typeof(Camera), "ProcessDisableMotor1", new[] { expression.Type }, expression);
                case CameraCommandType.DisableBothMotors:
                    return Expression.Call(typeof(Camera), "ProcessDisableBothMotors", new[] { expression.Type }, expression);

                /************************************************************************/
                /* Registers: CAM0_MMODE_PULSE and CAM1_MMODE_PULSE                     */
                /************************************************************************/
                case CameraCommandType.PositionMotor0:
                    if (expression.Type != typeof(UInt16)) { expression = Expression.Convert(expression, typeof(UInt16)); }
                    return Expression.Call(typeof(Camera), "ProcessPositionMotor0", null, expression);
                case CameraCommandType.PositionMotor1:
                    if (expression.Type != typeof(UInt16)) { expression = Expression.Convert(expression, typeof(UInt16)); }
                    return Expression.Call(typeof(Camera), "ProcessPositionMotor1", null, expression);
                    
                /************************************************************************/
                /* Registers: SET_OUTPUTS, CLR_OUTPUTS and OUTPUTS                      */
                /************************************************************************/
                case CameraCommandType.OutputTrig0:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Camera), "ProcessOutTrig0", null, expression);
                case CameraCommandType.OutputSync0:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Camera), "ProcessOutSync0", null, expression);
                case CameraCommandType.OutputTrig1:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Camera), "ProcessOutTrig1", null, expression);
                case CameraCommandType.OutputSync1:
                    if (expression.Type != typeof(bool)) { expression = Expression.Convert(expression, typeof(bool)); }
                    return Expression.Call(typeof(Camera), "ProcessOutSync1", null, expression);
                case CameraCommandType.Outputs:
                    if (expression.Type != typeof(byte)) { expression = Expression.Convert(expression, typeof(byte)); }
                    return Expression.Call(typeof(Camera), "ProcessOutputs", null, expression);

                default:
                    break;
            }
            return expression;
        }

        /************************************************************************/
        /* Registers: START_CAMS and STOP_CAMS                                  */
        /************************************************************************/
        static HarpDataFrame ProcessStartCamera0<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 32, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessStartCamera1<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 32, 255, (byte)HarpType.U8, 2, 0));
        }
        static HarpDataFrame ProcessStartBothCameras<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 32, 255, (byte)HarpType.U8, 3, 0));
        }

        static HarpDataFrame ProcessStopCamera0<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 33, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessStopCamera1<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 33, 255, (byte)HarpType.U8, 2, 0));
        }
        static HarpDataFrame ProcessStopBothCameras<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 33, 255, (byte)HarpType.U8, 3, 0));
        }

        /************************************************************************/
        /* Registers: ENABLE_MOTORS and DISABLE_MOTORS                          */
        /************************************************************************/
        static HarpDataFrame ProcessEnableMotor0<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 34, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessEnableMotor1<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 34, 255, (byte)HarpType.U8, 2, 0));
        }
        static HarpDataFrame ProcessEnableBothMotors<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 34, 255, (byte)HarpType.U8, 3, 0));
        }

        static HarpDataFrame ProcessDisableMotor0<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 35, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessDisableMotor1<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 35, 255, (byte)HarpType.U8, 2, 0));
        }
        static HarpDataFrame ProcessDisableBothMotors<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 35, 255, (byte)HarpType.U8, 3, 0));
        }

        /************************************************************************/
        /* Registers: CAM0_MMODE_PULSE and CAM1_MMODE_PULSE                     */
        /************************************************************************/
        static HarpDataFrame ProcessPositionMotor0(UInt16 input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 6, 52, 255, (byte)HarpType.U16, (byte)(input & 255), (byte)((input >> 8) & 255), 0));
        }
        static HarpDataFrame ProcessPositionMotor1(UInt16 input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 6, 56, 255, (byte)HarpType.U16, (byte)(input & 255), (byte)((input >> 8) & 255), 0));
        }

        /************************************************************************/
        /* Registers: SET_OUTPUTS, CLR_OUTPUTS and OUTPUTS                      */
        /************************************************************************/
        static HarpDataFrame ProcessOutTrig0(bool input)
        {
            if (input)
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 36, 255, (byte)HarpType.U8, 1, 0));
            else
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 37, 255, (byte)HarpType.U8, 1, 0));
        }
        static HarpDataFrame ProcessOutSync0(bool input)
        {
            if (input)
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 36, 255, (byte)HarpType.U8, 2, 0));
            else
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 37, 255, (byte)HarpType.U8, 2, 0));
        }
        static HarpDataFrame ProcessOutTrig1(bool input)
        {
            if (input)
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 36, 255, (byte)HarpType.U8, 4, 0));
            else
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 37, 255, (byte)HarpType.U8, 4, 0));
        }
        static HarpDataFrame ProcessOutSync1(bool input)
        {
            if (input)
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 36, 255, (byte)HarpType.U8, 8, 0));
            else
                return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 37, 255, (byte)HarpType.U8, 8, 0));
        }
        static HarpDataFrame ProcessOutputs(byte input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 38, 255, (byte)HarpType.U8, input, 0));
        }
    }
}
