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
    public enum PyControlLocationType : byte
    {
        Port1 = 0,
        Port2,
        Port3,
        Port4,
        Port5,
        Port6
    }

    public enum PyControlEventType : byte
    {
        /* Event: ANALOG */
        Analogs = 0,
        Bnc1,
        Bnc2,
        Dac1,
        Dac2,

        /* Event: PORTx */
        Port,
        PortDioA,
        PortDioB,
        PortPowerA,
        PortPowerB,
        PortPowerC,

        /* Event: PORTx */
        PokeIR,
        PokeLed,
        PokeSolenoid,

        /* Events: BUTTON and RESET */
        Button,
        Reset,

        /* Raw Registers */
        RegisterBnc1,
        RegisterBnc2,
        RegisterDac1,
        RegisterDac2,
        RegisterButton,
        RegisterReset
    }

        [Description(
        "\n" +
        "Analogs: Decimal Mat[4] (V)\n" +
        "Bnc1: Decimal (V)\n" +
        "Bnc2: Decimal (V)\n" +
        "Dac1: Decimal (V)\n" +
        "Dac2: Decimal (V)\n" +
        "\n" +
        "Port: Integer Mat[5]\n" +
        "PortDioA: Boolean (*)\n" +
        "PortDioB: Boolean (*)\n" +
        "PortPowerA: Boolean (*)\n" +
        "PortPowerB: Boolean (*)\n" +
        "PortPowerC: Boolean (*)\n" +
        "\n" +
        "Poke: Integer Mat[3]\n" +
        "PokeIR: Boolean (*)\n" +
        "PokeLed: Boolean (*)\n" +
        "PokeSolenoid: Boolean (*)\n" +
        "\n" +
        "Button: Boolean\n" +
        "Reset: Boolean\n" +
        "\n" +
        "RegisterBnc1,: U16\n" +
        "RegisterBnc2,: U16\n" +
        "RegisterDac1,: U16\n" +
        "RegisterDac2,: U16\n" +
        "RegisterButton,: U8\n" +
        "Registerreset,: U8\n" +
        "\n" +
        "(*) Only distinct contiguous elements are propagated."
    )]

    public class PyControl : SingleArgumentExpressionBuilder, INamedElement
    {
        public PyControl()
        {
            Type = PyControlEventType.Analogs;
            Location = PyControlLocationType.Port1;
        }

        string INamedElement.Name
        {
            get { return typeof(PyControl).Name + "." + Type.ToString(); }
        }

        public PyControlEventType Type { get; set; }
        public PyControlLocationType Location { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* Register: ANALOG                                                     */
                /************************************************************************/
                case PyControlEventType.Analogs:
                    return Expression.Call(typeof(PyControl), "ProcessAnalogs", null, expression);

                case PyControlEventType.Bnc1:
                    return Expression.Call(typeof(PyControl), "ProcessBNC1", null, expression);
                case PyControlEventType.Bnc2:
                    return Expression.Call(typeof(PyControl), "ProcessBNC2", null, expression);
                case PyControlEventType.Dac1:
                    return Expression.Call(typeof(PyControl), "ProcessDAC1", null, expression);
                case PyControlEventType.Dac2:
                    return Expression.Call(typeof(PyControl), "ProcessDAC2", null, expression);

                case PyControlEventType.RegisterBnc1:
                    return Expression.Call(typeof(PyControl), "ProcessRegisterBnc1", null, expression);
                case PyControlEventType.RegisterBnc2:
                    return Expression.Call(typeof(PyControl), "ProcessRegisterBnc2", null, expression);
                case PyControlEventType.RegisterDac1:
                    return Expression.Call(typeof(PyControl), "ProcessRegisterDac1", null, expression);
                case PyControlEventType.RegisterDac2:
                    return Expression.Call(typeof(PyControl), "ProcessRegisterDac2", null, expression);

                /************************************************************************/
                /* Register: PORTx                                                      */
                /************************************************************************/
                case PyControlEventType.Port:
                    return Expression.Call(typeof(PyControl), "ProcessPort", null, expression, GetLocationFilter());
                case PyControlEventType.PortDioA:
                    return Expression.Call(typeof(PyControl), "ProcessPortDioA", null, expression, GetLocationFilter());
                case PyControlEventType.PortDioB:
                    return Expression.Call(typeof(PyControl), "ProcessPortDioB", null, expression, GetLocationFilter());
                case PyControlEventType.PortPowerA:
                    return Expression.Call(typeof(PyControl), "ProcessPortPowerA", null, expression, GetLocationFilter());
                case PyControlEventType.PortPowerB:
                    return Expression.Call(typeof(PyControl), "ProcessPortPowerB", null, expression, GetLocationFilter());
                case PyControlEventType.PortPowerC:
                    return Expression.Call(typeof(PyControl), "ProcessPortPowerC", null, expression, GetLocationFilter());

                /************************************************************************/
                /* Register: PORTx                                                      */
                /************************************************************************/
                case PyControlEventType.PokeIR:
                    return Expression.Call(typeof(PyControl), "ProcessPokeIR", null, expression, GetLocationFilter());
                case PyControlEventType.PokeLed:
                    return Expression.Call(typeof(PyControl), "ProcessPokeLed", null, expression, GetLocationFilter());
                case PyControlEventType.PokeSolenoid:
                    return Expression.Call(typeof(PyControl), "ProcessPokeSolenoid", null, expression, GetLocationFilter());

                /************************************************************************/
                /* Events: BUTTON and RESET                                             */
                /************************************************************************/
                case PyControlEventType.Button:
                    return Expression.Call(typeof(PyControl), "ProcessButton", null, expression);
                case PyControlEventType.Reset:
                    return Expression.Call(typeof(PyControl), "ProcessReset", null, expression);

                case PyControlEventType.RegisterButton:
                    return Expression.Call(typeof(PyControl), "ProcessRegisterButton", null, expression);
                case PyControlEventType.RegisterReset:
                    return Expression.Call(typeof(PyControl), "ProcessRegisterReset", null, expression);

                /************************************************************************/
                /* Default                                                              */
                /************************************************************************/
                default:
                    throw new InvalidOperationException("Invalid selection or not supported yet.");
            }
        }

        Expression GetLocationFilter()
        {
            Func<HarpDataFrame, bool> filter;
            switch (Location)
            {

                case PyControlLocationType.Port1: filter = is_evt_location_Port1; break;
                case PyControlLocationType.Port2: filter = is_evt_location_Port2; break;
                case PyControlLocationType.Port3: filter = is_evt_location_Port3; break;
                case PyControlLocationType.Port4: filter = is_evt_location_Port4; break;
                case PyControlLocationType.Port5: filter = is_evt_location_Port5; break;
                case PyControlLocationType.Port6: filter = is_evt_location_Port6; break;
                default: filter = frame => false; break;
            }
            return Expression.Constant(filter);
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
        static bool is_evt35(HarpDataFrame input) { return ((input.Address == 35) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt36(HarpDataFrame input) { return ((input.Address == 36) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt37(HarpDataFrame input) { return ((input.Address == 37) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt38(HarpDataFrame input) { return ((input.Address == 38) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt39(HarpDataFrame input) { return ((input.Address == 39) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt40(HarpDataFrame input) { return ((input.Address == 40) && (input.Error == false) && (input.Id == MessageId.Event)); }

        static bool is_evt_location_Port1(HarpDataFrame input) { return ((input.Address == 35) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt_location_Port2(HarpDataFrame input) { return ((input.Address == 36) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt_location_Port3(HarpDataFrame input) { return ((input.Address == 37) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt_location_Port4(HarpDataFrame input) { return ((input.Address == 38) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt_location_Port5(HarpDataFrame input) { return ((input.Address == 39) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt_location_Port6(HarpDataFrame input) { return ((input.Address == 40) && (input.Error == false) && (input.Id == MessageId.Event)); }

        /************************************************************************/
        /* Register: ANALOG                                                     */
        /************************************************************************/
        static IObservable<Mat> ProcessAnalogs(IObservable<HarpDataFrame> source)
        {
            return Observable.Defer(() =>
            {
                var buffer = new float[4];
                return source.Where(is_evt34).Select(input =>
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = (float)((3.3 / 1.6) / 4096) * BitConverter.ToInt16(input.Message, 11 + i * 2);
                    }

                    return Mat.FromArray(buffer, 4, 1, Depth.F32, 1);
                });
            });
        }

        static IObservable<float> ProcessBNC1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return (float)((3.3 / 1.6) / 4096) * ((int)(UInt16)(BitConverter.ToUInt16(input.Message, 11))); });
        }
        static IObservable<float> ProcessBNC2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return (float)((3.3 / 1.6) / 4096) * ((int)(UInt16)(BitConverter.ToUInt16(input.Message, 13))); });
        }
        static IObservable<float> ProcessDAC1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return (float)((3.3 / 1.6) / 4096) * ((int)(UInt16)(BitConverter.ToUInt16(input.Message, 15))); });
        }
        static IObservable<float> ProcessDAC2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return (float)((3.3 / 1.6) / 4096) * ((int)(UInt16)(BitConverter.ToUInt16(input.Message, 17))); });
        }
        
        static IObservable<Timestamped<UInt16>> ProcessRegisterBnc1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterBnc2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 13), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterDac1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 15), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterDac2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 17), ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Register: PORTx                                                      */
        /************************************************************************/
        static IObservable<Mat> ProcessPort(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return Observable.Defer(() =>
            {
                var buffer = new byte[5];
                return source.Where(filter).Select(input =>
                {
                    var inputs = input.Message[11];

                    for (int i = 0; i < buffer.Length; i++)
                        buffer[i] = (byte)((inputs >> i) & 1);

                    return Mat.FromArray(buffer, 5, 1, Depth.U8, 1);
                });
            });
        }

        static IObservable<bool> ProcessPortDioA(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return source.Where(filter).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPortDioB(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return source.Where(filter).Select(input => { return ((input.Message[11] & (1 << 1)) == (1 << 1)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPortPowerA(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return source.Where(filter).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 2)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPortPowerB(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return source.Where(filter).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 3)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPortPowerC(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return source.Where(filter).Select(input => { return ((input.Message[11] & (1 << 4)) == (1 << 4)); }).DistinctUntilChanged();
        }

        static IObservable<bool> ProcessPokeIR(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return source.Where(filter).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPokeLed(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return source.Where(filter).Select(input => { return ((input.Message[11] & (1 << 2)) == (1 << 2)); }).DistinctUntilChanged();
        }
        static IObservable<bool> ProcessPokeSolenoid(IObservable<HarpDataFrame> source, Func<HarpDataFrame, bool> filter)
        {
            return source.Where(filter).Select(input => { return ((input.Message[11] & (1 << 3)) == (1 << 3)); }).DistinctUntilChanged();
        }

        static IObservable<Timestamped<UInt16>> ProcessRegisterPort1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt35).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterPort2(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt36).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterPort3(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt37).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterPort4(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt38).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterPort5(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt39).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterPort6(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt40).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Events: BUTTON and RESET                                             */
        /************************************************************************/
        static IObservable<bool> ProcessButton(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); });
        }
        static IObservable<bool> ProcessReset(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt33).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)); });
        }

        static IObservable<Timestamped<byte>> ProcessRegisterButton(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt32).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<byte>> ProcessRegisterReset(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt33).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }
    }
}
