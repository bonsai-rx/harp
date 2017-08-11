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

/* Events are divided into two categories: Bonsai Events and Raw Registers. */
/*   - Bonsai Events:                                                                                                                      */
/*                   Should follow Bonsai guidelines and use types like int, bool, float, Mat and string (for Enums like Wear's DEV_SELECT */
/*   - Raw Registers:                                                                                                                      */
/*                   Should have the Timestamped output and the value must have the exact same type of the Harp device register.           */
/*                   An exception can be made to the output type when:                                                                     */
/*                           1. The register only have one bit that can be considered as a pure boolean. Can use bool as ouput type.       */

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
                /* Event: INPUTS_STATE                                                  */
                /************************************************************************/
                case MultiPwmEventType.RegisterOutputs:
                    return Expression.Call(typeof(MultiPwm), "ProcessRegisterPwmOutputs", null, expression);

                /************************************************************************/
                /* Event: INPUTS_STATE (boolean and address)                            */
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
        /* Event: INPUTS_STATE                                                  */
        /************************************************************************/
        static IObservable<Timestamped<byte>> ProcessRegisterPwmOutputs(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt73).Select(input => {  return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: EXEC_STATE (boolean)                                          */
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
    }
}
