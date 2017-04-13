using Bonsai;
using Bonsai.Expressions;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
// TODO: replace this with the transform input and output types.
using TResult = System.String;

/* Events are divided into two categories: Bonsai Events and Raw Registers. */
/*   - Bonsai Events:                                                                                                                      */
/*                   Should follow Bonsai guidelines and use types like int, bool, float, Mat and string (for Enums like Arquimedes's DEV_SELECT */
/*   - Raw Registers:                                                                                                                      */
/*                   Should have the Timestamped output and the value must have the exact same type of the Harp device register.           */
/*                   An exception can be made to the output type when:                                                                     */
/*                           1. The register only have one bit that can be considered as a pure boolena. Can use bool as ouput type.       */

namespace Bonsai.Harp.Events
{
    public enum ArquimedesEventType : byte
    {
        /* Event: DATA */
        Lever = 0,
        AnalogInput,

        /* Event: THRESHOLDS */
        Thresholds,
        LeverIsQuiet,
        Threshold0,
        Threshold1,
        Threshold2,
        Threshold3,

        /* Event: INPUTS */
        Inputs,
        Input0,
        Input1,
        Input2,
        Input3,

        /* Event: POS_CURRENT */
        MotorPosition,

        /* Raw Registers */
        RegisterLever,
        RegisterAnalogInput,
        RegisterThresholds,
        RegisterInputs,
        RegisterMotorPosition,
    }

    public class Arquimedes : SingleArgumentExpressionBuilder, INamedElement
    {
        public Arquimedes()
        {
            Type = ArquimedesEventType.Lever;
        }

        string INamedElement.Name
        {
            get { return typeof(Arquimedes).Name + "." + Type.ToString(); }
        }

        public ArquimedesEventType Type { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* Event: DATA                                                          */
                /************************************************************************/
                case ArquimedesEventType.Lever:
                    return Expression.Call(typeof(Arquimedes), "ProcessLever", null, expression);
                case ArquimedesEventType.AnalogInput:
                    return Expression.Call(typeof(Arquimedes), "ProcessAnalogInput", null, expression);

                case ArquimedesEventType.RegisterLever:
                    return Expression.Call(typeof(Arquimedes), "ProcessRegisterLever", null, expression);
                case ArquimedesEventType.RegisterAnalogInput:
                    return Expression.Call(typeof(Arquimedes), "ProcessRegisterAnalogInput", null, expression);

                /************************************************************************/
                /* Event: THRESHOLDS                                                    */
                /************************************************************************/
                case ArquimedesEventType.Thresholds:
                    return Expression.Call(typeof(Arquimedes), "ProcessThresholds", null, expression);
                case ArquimedesEventType.LeverIsQuiet:
                    return Expression.Call(typeof(Arquimedes), "ProcessLeverIsQuiet", null, expression);
                case ArquimedesEventType.Threshold0:
                    return Expression.Call(typeof(Arquimedes), "ProcessThreshold0", null, expression);
                case ArquimedesEventType.Threshold1:
                    return Expression.Call(typeof(Arquimedes), "ProcessThreshold1", null, expression);
                case ArquimedesEventType.Threshold2:
                    return Expression.Call(typeof(Arquimedes), "ProcessThreshold2", null, expression);
                case ArquimedesEventType.Threshold3:
                    return Expression.Call(typeof(Arquimedes), "ProcessThreshold3", null, expression);

                case ArquimedesEventType.RegisterThresholds:
                    return Expression.Call(typeof(Arquimedes), "ProcessRegisterThresholds", null, expression);

                /************************************************************************/
                /* Event: INPUTS                                                          */
                /************************************************************************/
                case ArquimedesEventType.Inputs:
                    return Expression.Call(typeof(Arquimedes), "ProcessInputs", null, expression);
                case ArquimedesEventType.Input0:
                    return Expression.Call(typeof(Arquimedes), "ProcessInputs0", null, expression);
                case ArquimedesEventType.Input1:
                    return Expression.Call(typeof(Arquimedes), "ProcessInputs1", null, expression);
                case ArquimedesEventType.Input2:
                    return Expression.Call(typeof(Arquimedes), "ProcessInputs2", null, expression);
                case ArquimedesEventType.Input3:
                    return Expression.Call(typeof(Arquimedes), "ProcessInputs3", null, expression);

                case ArquimedesEventType.RegisterInputs:
                    return Expression.Call(typeof(Arquimedes), "ProcessRegisterInputs", null, expression);

                /************************************************************************/
                /* Event: POS_CURRENT                                                   */
                /************************************************************************/
                case ArquimedesEventType.MotorPosition:
                    return Expression.Call(typeof(Arquimedes), "ProcessPosition", null, expression);

                case ArquimedesEventType.RegisterMotorPosition:
                    return Expression.Call(typeof(Arquimedes), "ProcesssMotorPosition", null, expression);

                /************************************************************************/
                /* Default                                                              */
                /************************************************************************/
                default:
                    throw new InvalidOperationException("Invalid selection or not supported yet.");
            }
        }

        static double ParseTimestamp(byte[] message, int index)
        {
            var seconds = BitConverter.ToUInt32(message, index);
            var microseconds = BitConverter.ToUInt16(message, index + 4);
            return seconds + microseconds * 32e-6;
        }

        static bool is_evt32(HarpDataFrame input) { return ((input.Address == 32) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt33(HarpDataFrame input) { return ((input.Address == 33) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt34(HarpDataFrame input) { return ((input.Address == 34) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt55(HarpDataFrame input) { return ((input.Address == 34) && (input.Error == false) && (input.Id == MessageId.Event)); }

        /************************************************************************/
        /* Event: DATA                                                          */
        /************************************************************************/
        static IObservable<Mat> Processlever(IObservable<HarpDataFrame> source)
        {
            return Observable.Defer(() =>
            {
                var buffer = new float[1];
                return source.Where(is_evt33).Select(input =>
                {
                    buffer[0] = BitConverter.ToInt16(input.Message, 11) * (float)0.0219;

                    return Mat.FromArray(buffer, 1, 1, Depth.F32, 1);
                });
            });
        }
        static IObservable<Mat> ProcessAnalogInput(IObservable<HarpDataFrame> source)
        {
            return Observable.Defer(() =>
            {
                var buffer = new ushort[1];
                return source.Where(is_evt33).Select(input =>
                {
                    buffer[0] = BitConverter.ToUInt16(input.Message, 13);

                    return Mat.FromArray(buffer, 1, 1, Depth.U16, 1);
                });
            });
        }


        static IObservable<Timestamped<Int16>> ProcessRegisterLever(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt33).Select(input => { return new Timestamped<Int16>(BitConverter.ToInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<Int16>> ProcessRegisterAnalogInput(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt33).Select(input => { return new Timestamped<Int16>(BitConverter.ToInt16(input.Message, 13), ParseTimestamp(input.Message, 5)); });
        }
        
        /************************************************************************/
        /* Event: THRESHOLDS                                                    */
        /************************************************************************/
        static IObservable<Mat> ProcessThresholds(IObservable<HarpDataFrame> source)
        {
            return Observable.Defer(() =>
            {
                var buffer = new byte[5];
                return source.Where(is_evt32).Select(input =>
                {
                    for (int i = 0; i < buffer.Length; i++)
                        buffer[i] = (byte)((input.Message[11] >> i) & 1);

                    return Mat.FromArray(buffer, buffer.Length, 1, Depth.U8, 1);
                });
            });
        }

        static IObservable<bool> ProcessLeverIsQuiet(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); });
        }
        static IObservable<bool> ProcessThreshold0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 1)) == (1 << 0)); });
        }
        static IObservable<bool> ProcessThreshold1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 1)); });
        }
        static IObservable<bool> ProcessThreshold2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 2)); });
        }
        static IObservable<bool> ProcessThreshold3(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 4)) == (1 << 3)); });
        }

        static IObservable<Timestamped<byte>> ProcessRegisterThresholds(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: INPUTS                                                        */
        /************************************************************************/
        static IObservable<Mat> ProcessInputs(IObservable<HarpDataFrame> source)
        {
            return Observable.Defer(() =>
            {
                var buffer = new byte[4];
                return source.Where(is_evt34).Select(input =>
                {
                    for (int i = 0; i < buffer.Length; i++)
                        buffer[i] = (byte)((input.Message[11] >> i) & 1);

                    return Mat.FromArray(buffer, buffer.Length, 1, Depth.U8, 1);
                });
            });
        }

        static IObservable<bool> ProcessInput0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); });
        }
        static IObservable<bool> ProcessInput1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return ((input.Message[11] & (1 << 1)) == (1 << 0)); });
        }
        static IObservable<bool> ProcessInput2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 1)); });
        }
        static IObservable<bool> ProcessInput3(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 1)); });
        }
        
        static IObservable<Timestamped<byte>> ProcessRegisterInputs(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: POS_CURRENT                                                   */
        /************************************************************************/
        static IObservable<Mat> ProcessMotorPosition(IObservable<HarpDataFrame> source)
        {
            return Observable.Defer(() =>
            {
                var buffer = new ushort[1];
                return source.Where(is_evt55).Select(input =>
                {
                    buffer[0] = BitConverter.ToUInt16(input.Message, 11);

                    return Mat.FromArray(buffer, 1, 1, Depth.U16, 1);
                });
            });
        }

        static IObservable<Timestamped<UInt16>> ProcessRegisterMotorPosition(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt55).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
    }
}
