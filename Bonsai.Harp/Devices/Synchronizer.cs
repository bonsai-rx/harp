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

namespace Bonsai.Harp.Devices
{
    public class Synchronizer : SingleArgumentExpressionBuilder
    {
        public Synchronizer()
        {
            Type = SynchronizerEvent.EVT0_Inputs;
        }

        public SynchronizerEvent Type { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* Event: INPUTS_STATE                                                  */
                /************************************************************************/
                case SynchronizerEvent.EVT0_Inputs:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Inputs", null, expression);
                case SynchronizerEvent.EVT0_InputsRaw:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_InputsRaw", null, expression);

                /************************************************************************/
                /* Event: INPUTS_STATE (boolean and address)                            */
                /************************************************************************/
                case SynchronizerEvent.EVT0_Input0:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input0", null, expression);
                case SynchronizerEvent.EVT0_Input1:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input1", null, expression);
                case SynchronizerEvent.EVT0_Input2:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input2", null, expression);
                case SynchronizerEvent.EVT0_Input3:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input3", null, expression);
                case SynchronizerEvent.EVT0_Input4:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input4", null, expression);
                case SynchronizerEvent.EVT0_Input5:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input5", null, expression);
                case SynchronizerEvent.EVT0_Input6:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input6", null, expression);
                case SynchronizerEvent.EVT0_Input7:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input7", null, expression);
                case SynchronizerEvent.EVT0_Input8:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Input8", null, expression);
                case SynchronizerEvent.EVT0_Output0:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Output0", null, expression);
                case SynchronizerEvent.EVT0_Address:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT0_Address", null, expression);

                /************************************************************************/
                /* Event: OUTPUTS                                                       */
                /************************************************************************/
                case SynchronizerEvent.EVT1_Outputs:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT1_Outputs", null, expression);
                case SynchronizerEvent.EVT1_OutputsRaw:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT1_OutputsRaw", null, expression);

                /************************************************************************/
                /* Event: OUTPUTS (boolean)                                             */
                /************************************************************************/
                case SynchronizerEvent.EVT1_Output0:
                    return Expression.Call(typeof(Synchronizer), "ProcessEVT1_Output0", null, expression);

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

        static bool is_evt0(HarpDataFrame input) { return (input.Address == 32); }
        static bool is_evt1(HarpDataFrame input) { return (input.Address == 33); }

        /************************************************************************/
        /* Event: INPUTS_STATE                                                  */
        /************************************************************************/
        static IObservable<Mat> ProcessEVT0_Inputs(IObservable<HarpDataFrame> source)
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

        static Timestamped<UInt16> ProcessEVT0_InputsRaw(HarpDataFrame input)
        {
            var timestamp = ParseTimestamp(input.Message, 5);
            var value = BitConverter.ToUInt16(input.Message, 11);
            return new Timestamped<UInt16>(value, timestamp);
        }

        /************************************************************************/
        /* Event: INPUTS_STATE (boolean and address)                            */
        /************************************************************************/
        static bool ProcessEVT0_Input0(HarpDataFrame input) { return ((input.Message[11] & (1 << 0)) == (1 << 0)); }
        static bool ProcessEVT0_Input1(HarpDataFrame input) { return ((input.Message[11] & (1 << 1)) == (1 << 1)); }
        static bool ProcessEVT0_Input2(HarpDataFrame input) { return ((input.Message[11] & (1 << 2)) == (1 << 2)); }
        static bool ProcessEVT0_Input3(HarpDataFrame input) { return ((input.Message[11] & (1 << 3)) == (1 << 3)); }
        static bool ProcessEVT0_Input4(HarpDataFrame input) { return ((input.Message[11] & (1 << 4)) == (1 << 4)); }
        static bool ProcessEVT0_Input5(HarpDataFrame input) { return ((input.Message[11] & (1 << 5)) == (1 << 5)); }
        static bool ProcessEVT0_Input6(HarpDataFrame input) { return ((input.Message[11] & (1 << 6)) == (1 << 6)); }
        static bool ProcessEVT0_Input7(HarpDataFrame input) { return ((input.Message[11] & (1 << 7)) == (1 << 7)); }
        static bool ProcessEVT0_Input8(HarpDataFrame input) { return ((input.Message[12] & (1 << 0)) == (1 << 0)); }

        static bool ProcessEVT0_Output0(HarpDataFrame input) { return ((input.Message[12] & (1 << 5)) == (1 << 5)); }

        static int ProcessEVT0_Address(HarpDataFrame input)
        {
            //if (input.Address == 1)
            //    return;

            return (input.Message[12] >> 6) & 3;
        }

        /************************************************************************/
        /* Event: OUTPUTS                                                       */
        /************************************************************************/
        static Mat ProcessEVT1_Outputs(HarpDataFrame input)
        {
            if (!is_evt1(input))
                return null;

            var output = new Mat(1, 1, Depth.U8, 1);

            output.SetReal(0, (input.Message[11] & 1));

            return output;
        }

        static Timestamped<byte> ProcessEVT1_OutputsRaw(HarpDataFrame input)
        {
            var timestamp = ParseTimestamp(input.Message, 5);
            return new Timestamped<byte>(input.Message[11], timestamp);
        }

        /************************************************************************/
        /* Event: OUTPUTS (boolean)                                             */
        /************************************************************************/
        static bool ProcessEVT1_Output0(HarpDataFrame input) { return ((input.Message[11] & (1 << 8)) == (1 << 8)); }
    }
}
