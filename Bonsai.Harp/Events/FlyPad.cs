using Bonsai;
using Bonsai.Expressions;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using TResult = System.String;
using System.ComponentModel;

namespace Bonsai.Harp.Events
{
    public enum FlyPadEventType : byte
    {
        /* Event: DATA */
        SensorsData = 0,

        /* Event: DIGITAL_IN */
        DigitalIn,

        /* Raw Registers */
        RegisterDigitalIn,
        RegisterDigitalOut
    }

    [Description(
        "\n" +
        "SensorsData: Integer Mat[96]\n" +
        "\n" +
        "DigitalIn: Boolean\n" +
        "\n" +
        "RegisterDigitalIn: U8\n" +
        "RegisterDigitalOut: U8\n"
    )]

    public class FlyPad : SingleArgumentExpressionBuilder, INamedElement
    {
        public FlyPad()
        {
            Type = FlyPadEventType.SensorsData;
        }

        string INamedElement.Name
        {
            get { return typeof(FlyPad).Name + "." + Type.ToString(); }
        }

        public FlyPadEventType Type { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* Register: DATA                                                       */
                /************************************************************************/
                case FlyPadEventType.SensorsData:
                    return Expression.Call(typeof(FlyPad), "ProcessSensorsData", null, expression);                    

                /************************************************************************/
                /* Register: DIGITAL_IN                                                 */
                /************************************************************************/
                case FlyPadEventType.DigitalIn:
                    return Expression.Call(typeof(FlyPad), "ProcessDigitalIn", null, expression);

                /************************************************************************/
                /* Register: DIGITAL_IN, DIGITAL_OUT                                    */
                /************************************************************************/
                case FlyPadEventType.RegisterDigitalIn:
                    return Expression.Call(typeof(FlyPad), "ProcessRegisterDigitalIn", null, expression);
                case FlyPadEventType.RegisterDigitalOut:
                    return Expression.Call(typeof(FlyPad), "ProcessRegisterDigitalOut", null, expression);

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

        static bool is_evt34(HarpDataFrame input) { return ((input.Address == 34) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt35(HarpDataFrame input) { return ((input.Address == 35) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt38(HarpDataFrame input) { return ((input.Address == 38) && (input.Error == false) && (input.Id == MessageId.Event)); }

        /************************************************************************/
        /* Register: DATA                                                       */
        /************************************************************************/
        static IObservable<Mat> ProcessSensorsData(IObservable<HarpDataFrame> source)
        {
            return Observable.Defer(() =>
            {
                var buffer = new ushort[96];
                return source.Where(is_evt38).Select(input =>
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = BitConverter.ToUInt16(input.Message, 11 + i * 2);
                    }

                    return Mat.FromArray(buffer, 96, 1, Depth.U16, 1);
                });
            });
        }

        /************************************************************************/
        /* Register: DIGITAL_IN                                                 */
        /************************************************************************/
        static IObservable<bool> ProcessDigitalIn(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); });
        }

        /************************************************************************/
        /* Register: DIGITAL_IN, DIGITAL_OUT                                    */
        /************************************************************************/
        static IObservable<Timestamped<byte>> ProcessRegisterDigitalIn(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<byte>> ProcessRegisterDigitalOut(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt35).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }
    }
}
