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
/*                           1. The register only have one bit that can be considered as a pure boolena. Can use bool as ouput type.       */

namespace Bonsai.Harp.Events
{
    public class Synchronizer : SingleArgumentExpressionBuilder, INamedElement
    {
        public Synchronizer()
        {
            Type = EventType.Inputs;
        }

        public enum EventType : byte
        {
            /* Event: INPUTS_STATE */
            Inputs = 0,
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

            RegisterInputs,
        }

        string INamedElement.Name
        {
            get { return typeof(Synchronizer).Name + "." + Type.ToString(); }
        }

        public EventType Type { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* Event: INPUTS_STATE                                                  */
                /************************************************************************/
                case EventType.Inputs:
                    return Expression.Call(typeof(Synchronizer), "ProcessInputs", null, expression);
                case EventType.RegisterInputs:
                    return Expression.Call(typeof(Synchronizer), "ProcessRegisterInputs", null, expression);

                /************************************************************************/
                /* Event: INPUTS_STATE (boolean and address)                            */
                /************************************************************************/
                case EventType.Input0:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput0", null, expression);
                case EventType.Input1:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput1", null, expression);
                case EventType.Input2:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput2", null, expression);
                case EventType.Input3:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput3", null, expression);
                case EventType.Input4:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput4", null, expression);
                case EventType.Input5:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput5", null, expression);
                case EventType.Input6:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput6", null, expression);
                case EventType.Input7:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput7", null, expression);
                case EventType.Input8:
                    return Expression.Call(typeof(Synchronizer), "ProcessInput8", null, expression);
                case EventType.Output0:
                    return Expression.Call(typeof(Synchronizer), "ProcessOutput0", null, expression);
                case EventType.Address:
                    return Expression.Call(typeof(Synchronizer), "ProcessAddress", null, expression);

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

        /************************************************************************/
        /* Event: INPUTS_STATE                                                  */
        /************************************************************************/
        static IObservable<Mat> ProcessInputs(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input =>
            {
                var inputs = BitConverter.ToUInt16(input.Message, 11);
                var output = new Mat(10, 1, Depth.U8, 1);

                output.SetReal(9, (inputs >> 13) & 1);      // Output0

                for (int i = 0; i < output.Rows - 1; i++)
                    output.SetReal(i, (inputs >> i) & 1);

                return output;
            });
        }

        static IObservable<Timestamped<UInt16>> ProcessRegisterInputs(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => {  return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: INPUTS_STATE (boolean and address)                            */
        /************************************************************************/
        static IObservable<bool> ProcessInput0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); });
        }
        static IObservable<bool> ProcessInput1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 1)) == (1 << 1)); });
        }
        static IObservable<bool> ProcessInput2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 2)); });
        }
        static IObservable<bool> ProcessInput3(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 3)); });
        }
        static IObservable<bool> ProcessInput4(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 4)) == (1 << 4)); });
        }
        static IObservable<bool> ProcessInput5(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 5)) == (1 << 5)); });
        }
        static IObservable<bool> ProcessInput6(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 6)) == (1 << 6)); });
        }
        static IObservable<bool> ProcessInput7(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 7)) == (1 << 7)); });
        }
        static IObservable<bool> ProcessInput8(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[11] & (1 << 8)) == (1 << 8)); });
        }

        static IObservable<bool> ProcessOutput0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return ((input.Message[12] & (1 << 5)) == (1 << 5)); });
        }

        static IObservable<int> ProcessAddress(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt0).Select(input => { return (input.Message[12] >> 6) & 3; });
        }
    }
}
