using OpenCV.Net;
using Bonsai;
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
// TODO: replace this with the transform input and output types.
using TResult = System.String;

namespace Bonsai.Harp.Events
{
    public class Wear : SingleArgumentExpressionBuilder, INamedElement
    {
        public Wear()
        {
            Type = EventType.Motion;
        }

        public enum EventType : byte
        {
            /* Event: DATA */
            Motion = 0,
            MotionRegister,


            /* To code */
            Input0,
            Input1,
            Input2,
            Input3,
            Input4,
            Input5,
            Input6,
            Input7,
            Input8,
            Output0,
            Address,
        }

        string INamedElement.Name
        {
            get { return typeof(Wear).Name + "." + Type.ToString(); }
        }

        public EventType Type { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* Event: DATA                                                  */
                /************************************************************************/
                case EventType.Motion:
                    return Expression.Call(typeof(Wear), "ProcessMotion", null, expression);
                case EventType.MotionRegister:
                    return Expression.Call(typeof(Wear), "ProcessMotionRegister", null, expression);

                /************************************************************************/
                /* Event: INPUTS_STATE (boolean and address)                            */
                /************************************************************************/
                case EventType.Input0:
                    return Expression.Call(typeof(Wear), "ProcessInput0", null, expression);
                case EventType.Input1:
                    return Expression.Call(typeof(Wear), "ProcessInput1", null, expression);
                case EventType.Input2:
                    return Expression.Call(typeof(Wear), "ProcessInput2", null, expression);
                case EventType.Input3:
                    return Expression.Call(typeof(Wear), "ProcessInput3", null, expression);
                case EventType.Input4:
                    return Expression.Call(typeof(Wear), "ProcessInput4", null, expression);
                case EventType.Input5:
                    return Expression.Call(typeof(Wear), "ProcessInput5", null, expression);
                case EventType.Input6:
                    return Expression.Call(typeof(Wear), "ProcessInput6", null, expression);
                case EventType.Input7:
                    return Expression.Call(typeof(Wear), "ProcessInput7", null, expression);
                case EventType.Input8:
                    return Expression.Call(typeof(Wear), "ProcessInput8", null, expression);
                case EventType.Output0:
                    return Expression.Call(typeof(Wear), "ProcessOutput0", null, expression);
                case EventType.Address:
                    return Expression.Call(typeof(Wear), "ProcessAddress", null, expression);

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

        static bool is_evt0(HarpDataFrame input) { return ((input.Address == 32) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt34(HarpDataFrame input) { return ((input.Address == 34) && (input.Error == false) && (input.Id == MessageId.Event)); }

        /************************************************************************/
        /* Event: DATA                                                          */
        /************************************************************************/
        static IObservable<Mat> ProcessMotion(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input =>
            {
                var output = new Mat(9, 1, Depth.S16, 1);

                for (int i = 0; i < output.Rows; i++)
                    output.SetReal(i, BitConverter.ToInt16(input.Message, 11 + i * 2));

                return output;
            });
        }

        static IObservable<Timestamped<string>> ProcessMotionRegister(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input =>
            {
                var timestamp = ParseTimestamp(input.Message, 5);
                var value = BitConverter.ToUInt16(input.Message, 11);
                return new Timestamped<string>("teste", timestamp);
            });
        }

        /************************************************************************/
        /* Event: INPUTS_STATE (boolean and address)                            */
        /************************************************************************/
        static IObservable<bool> ProcessInput0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 0)) == (1 << 0));
            });
        }

        static IObservable<bool> ProcessInput1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 1)) == (1 << 1));
            });
        }

        static IObservable<bool> ProcessInput2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 2)) == (1 << 2));
            });
        }

        static IObservable<bool> ProcessInput3(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 3)) == (1 << 3));
            });
        }

        static IObservable<bool> ProcessInput4(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 4)) == (1 << 4));
            });
        }

        static IObservable<bool> ProcessInput5(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 5)) == (1 << 5));
            });
        }

        static IObservable<bool> ProcessInput6(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 6)) == (1 << 6));
            });
        }

        static IObservable<bool> ProcessInput7(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 7)) == (1 << 7));
            });
        }

        static IObservable<bool> ProcessInput8(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[11] & (1 << 8)) == (1 << 8));
            });
        }

        static IObservable<bool> ProcessOutput0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return ((input.Message[12] & (1 << 5)) == (1 << 5));
            });
        }

        static IObservable<int> ProcessAddress(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                return (input.Message[12] >> 6) & 3;
            });
        }
    }
}
