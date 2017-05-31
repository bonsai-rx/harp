﻿using Bonsai;
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
/*                   Should follow Bonsai guidelines and use types like int, bool, float, Mat and string (for Enums like Wear's DEV_SELECT */
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

        TransitionQuiet,
        TransitionThreshold0,
        TransitionThreshold1,
        TransitionThreshold2,
        TransitionThreshold3,

        /* Event: INPUTS */
        Inputs,
        Input0,
        Input1,
        Input2,
        Input3,

        /* Event: POS_CURRENT */
        LoadPositionPercentage,

        /* Raw Registers */
        RegisterLever,
        RegisterAnalogInput,
        RegisterThresholds,
        RegisterInputs,
        RegisterLoadPosition,
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

        private static byte previousThresholdsQuiet = 0;
        private static byte previousThresholdsTh0 = 0;
        private static byte previousThresholdsTh1 = 0;
        private static byte previousThresholdsTh2 = 0;
        private static byte previousThresholdsTh3 = 0;

        private static bool REG_MOTOR_MAXIMUM_available = false;
        private static UInt16 REG_MOTOR_MAXIMUM = 3000;

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
                    
                case ArquimedesEventType.TransitionQuiet:
                    return Expression.Call(typeof(Arquimedes), "ProcessTransitionQuiet", null, expression);
                case ArquimedesEventType.TransitionThreshold0:
                    return Expression.Call(typeof(Arquimedes), "ProcessTransitionThreshold0", null, expression);
                case ArquimedesEventType.TransitionThreshold1:
                    return Expression.Call(typeof(Arquimedes), "ProcessTransitionThreshold1", null, expression);
                case ArquimedesEventType.TransitionThreshold2:
                    return Expression.Call(typeof(Arquimedes), "ProcessTransitionThreshold2", null, expression);
                case ArquimedesEventType.TransitionThreshold3:
                    return Expression.Call(typeof(Arquimedes), "ProcessTransitionThreshold3", null, expression);

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
                case ArquimedesEventType.LoadPositionPercentage:
                    return Expression.Call(typeof(Arquimedes), "ProcessLoadPositionPercentage", null, expression);

                case ArquimedesEventType.RegisterLoadPosition:
                    return Expression.Call(typeof(Arquimedes), "ProcessRegisterLoadPosition", null, expression);

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
        static bool is_evt55(HarpDataFrame input)
        {
            if (input.Address == 60)
            {
                REG_MOTOR_MAXIMUM = BitConverter.ToUInt16(input.Message, 11);
                REG_MOTOR_MAXIMUM_available = true;
            }

            if (REG_MOTOR_MAXIMUM_available == false)
                return false;

            return ((input.Address == 55) && (input.Error == false) && (input.Id == MessageId.Event));
        }

        static bool compareWithPreviousThreshold(byte thresholds, ref byte previousThreshold, byte thresholdPosition)
        {
            bool current  = ((thresholds & (1 << thresholdPosition)) == (1 << thresholdPosition));
            bool previous = ((previousThreshold & (1 << thresholdPosition)) == (1 << thresholdPosition));

            if (current != previous)
            {
                previousThreshold = thresholds;
                return true;
            }
            else
            {
                previousThreshold = thresholds;
                return false;
            }
        }

        static bool xmit_quiet(HarpDataFrame input) { if (!is_evt32(input)) return false; return compareWithPreviousThreshold(input.Message[11], ref previousThresholdsQuiet, 0); }
        static bool xmit_th0  (HarpDataFrame input) { if (!is_evt32(input)) return false; return compareWithPreviousThreshold(input.Message[11], ref previousThresholdsTh0, 1); }
        static bool xmit_th1  (HarpDataFrame input) { if (!is_evt32(input)) return false; return compareWithPreviousThreshold(input.Message[11], ref previousThresholdsTh1, 2); }
        static bool xmit_th2  (HarpDataFrame input) { if (!is_evt32(input)) return false; return compareWithPreviousThreshold(input.Message[11], ref previousThresholdsTh2, 3); }
        static bool xmit_th3  (HarpDataFrame input) { if (!is_evt32(input)) return false; return compareWithPreviousThreshold(input.Message[11], ref previousThresholdsTh3, 4); }

        /************************************************************************/
        /* Event: DATA                                                          */
        /************************************************************************/
        static IObservable<float> Processlever(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt33).Select(input => { return BitConverter.ToInt16(input.Message, 11) * (float)0.0219; });
        }
        static IObservable<float> ProcessAnalogInput(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt33).Select(input => { return (float)((3.3 / 1.6) / 2048) * BitConverter.ToUInt16(input.Message, 13); });
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
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 1)) == (1 << 1)); });
        }
        static IObservable<bool> ProcessThreshold1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 2)); });
        }
        static IObservable<bool> ProcessThreshold2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 3)); });
        }
        static IObservable<bool> ProcessThreshold3(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 4)) == (1 << 4)); });
        }

        static IObservable<bool> ProcessTransitionQuiet(IObservable<HarpDataFrame> source)
        {
            return source.Where(/*is_evt32*/xmit_quiet).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); });
        }
        static IObservable<bool> ProcessTransitionThreshold0(IObservable<HarpDataFrame> source)
        {
            return source.Where(/*is_evt32*/xmit_th0).Select(input => { return ((input.Message[11] & (1 << 1)) == (1 << 1)); });
        }
        static IObservable<bool> ProcessTransitionThreshold1(IObservable<HarpDataFrame> source)
        {
            return source.Where(/*is_evt32*/xmit_th1).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 2)); });
        }
        static IObservable<bool> ProcessTransitionThreshold2(IObservable<HarpDataFrame> source)
        {
            return source.Where(/*is_evt32*/xmit_th2).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 3)); });
        }
        static IObservable<bool> ProcessTransitionThreshold3(IObservable<HarpDataFrame> source)
        {
            return source.Where(/*is_evt32*/xmit_th3).Select(input => { return ((input.Message[11] & (1 << 4)) == (1 << 4)); });
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
            return source.Where(is_evt34).Select(input => { return ((input.Message[11] & (1 << 1)) == (1 << 1)); });
        }
        static IObservable<bool> ProcessInput2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 2)); });
        }
        static IObservable<bool> ProcessInput3(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 3)); });
        }
        
        static IObservable<Timestamped<byte>> ProcessRegisterInputs(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: POS_CURRENT                                                   */
        /************************************************************************/
        static IObservable<float> ProcessLoadPositionPercentage(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt55).Select(input => { return ((float)(BitConverter.ToUInt16(input.Message, 11)) / (float)REG_MOTOR_MAXIMUM) * (float)100.0; });
        }

        static IObservable<Timestamped<UInt16>> ProcessRegisterLoadPosition(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt55).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
    }
}