using OpenCV.Net;
using Bonsai;
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Text;
using TResult = System.String;

namespace Bonsai.Harp.Events
{
    public enum MultiPwmEventType : byte
    {
        /* Event: INPUTS_STATE */
        Output0 = 0,
        Output1,
        Output2,
        Output3,

        RegisterOutputs,
    }

    [Description(
        "\n" +
        "Output0: Boolean (*)\n" +
        "Output1: Boolean (*)\n" +
        "Output2: Boolean (*)\n" +
        "Output3: Boolean (*)\n" +
        "\n" +
        "RegisterOutputs: Bitmask U8\n" +
        "\n" +
        "(*) Only distinct contiguous elements are propagated."
    )]

    public class MultiPwm : SingleArgumentExpressionBuilder, INamedElement
    {
        public MultiPwm()
        {
            Type = MultiPwmEventType.Output0;
        }

        string INamedElement.Name
        {
            get { return typeof(MultiPwm).Name + "." + Type.ToString(); }
        }

        public MultiPwmEventType Type { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* Register: EXEC_STATE                                                 */
                /************************************************************************/
                case MultiPwmEventType.RegisterOutputs:
                    return Expression.Call(typeof(MultiPwm), "ProcessRegisterPwmOutputs", null, expression);

                /************************************************************************/
                /* Register: EXEC_STATE                                                 */
                /************************************************************************/
                case MultiPwmEventType.Output0:
                    return Expression.Call(typeof(MultiPwm), "ProcessPwmOutput0", null, expression);
                case MultiPwmEventType.Output1:
                    return Expression.Call(typeof(MultiPwm), "ProcessPwmOutput1", null, expression);
                case MultiPwmEventType.Output2:
                    return Expression.Call(typeof(MultiPwm), "ProcessPwmOutput2", null, expression);
                case MultiPwmEventType.Output3:
                    return Expression.Call(typeof(MultiPwm), "ProcessPwmOutput3", null, expression);

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

        static bool is_evt73(HarpDataFrame input) { return ((input.Address == 73) && (input.Error == false) && (input.Id == MessageId.Event)); }

        /************************************************************************/
        /* Register: EXEC_STATE                                                 */
        /************************************************************************/
        static IObservable<bool> ProcessPwmOutput0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt73).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPwmOutput1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt73).Select(input => { return ((input.Message[11] & (1 << 1)) == (1 << 1)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPwmOutput2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt73).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 2)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPwmOutput3(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt73).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 3)); }).DistinctUntilChanged();
        }

        /************************************************************************/
        /* Register: EXEC_STATE                                                 */
        /************************************************************************/
        static IObservable<Timestamped<byte>> ProcessRegisterPwmOutputs(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt73).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }
    }
}
