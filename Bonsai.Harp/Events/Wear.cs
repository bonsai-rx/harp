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
/*                   Should follow Bonsai guidelines and use types like int, bool, float, Mat and string (for Enums like Wear's DEV_SELECT */
/*   - Raw Registers:                                                                                                                      */
/*                   Should have the Timestamped output and the value must have the exact same type of the Harp device register.           */
/*                   An exception can be made to the output type when:                                                                     */
/*                           1. The register only have one bit that can be considered as a pure boolena. Can use bool as ouput type.       */

namespace Bonsai.Harp.Events
{
    public class Wear : SingleArgumentExpressionBuilder, INamedElement
    {
        public Wear()
        {
            Type = EventType.MotionAll;
        }

        public enum EventType : byte
        {            
            /* Event: DATA */
            MotionAll = 0,
            MotionAccelerometer,
            MotionGyroscope,
            MotionMagnetometer,

            /* Event: MISC */
            AnalogInput,
            DigitalInput0,
            DigitalInput1,

            /* Event: ACQ_STATUS */
            Acquiring,

            /* Event: DEV_SELECT */
            DeviceSelected,

            /* Event: BATTERY, TX_RETRIES and RX_GOOD */
            SensorTemperature,
            TxRetries,
            Battery,
            RxGood,

            /* Raw Registers */
            RegisterStimulationStart,
            RegisterMisc,
            RegisterCamera0,
            RegisterCamera1,
            RegisterAcquisitionStatus,
            RegisterDeviceSelected,
            RegisterSensorTemperature,
            RegisterTxRetries,
            RegisterBattery,
            RegisterRxGood,
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
                /* Event: DATA                                                          */
                /************************************************************************/
                case EventType.MotionAll:
                    return Expression.Call(typeof(Wear), "ProcessMotionAll", null, expression);
                case EventType.MotionAccelerometer:
                    return Expression.Call(typeof(Wear), "ProcessMotionAccelerometer", null, expression);
                case EventType.MotionGyroscope:
                    return Expression.Call(typeof(Wear), "ProcessMotionGyroscope", null, expression);
                case EventType.MotionMagnetometer:
                    return Expression.Call(typeof(Wear), "ProcessMotionMagnetometer", null, expression);
                
                /************************************************************************/
                /* Event: MISC                                                          */
                /************************************************************************/
                case EventType.AnalogInput:
                    return Expression.Call(typeof(Wear), "ProcessAnalogInput", null, expression);
                case EventType.DigitalInput0:
                    return Expression.Call(typeof(Wear), "ProcessDigitalInput0", null, expression);
                case EventType.DigitalInput1:
                    return Expression.Call(typeof(Wear), "ProcessDigitalInput1", null, expression);


                case EventType.RegisterMisc:
                    return Expression.Call(typeof(Wear), "ProcessRegisterMisc", null, expression);

                /************************************************************************/
                /* Event: ACQ_STATUS                                                    */
                /************************************************************************/
                case EventType.Acquiring:
                    return Expression.Call(typeof(Wear), "ProcessAcquiring", null, expression);
                case EventType.RegisterAcquisitionStatus:
                    return Expression.Call(typeof(Wear), "ProcessRegisterAcquisitionStatus", null, expression);

                /************************************************************************/
                /* Event: START_STIM                                                    */
                /************************************************************************/
                case EventType.RegisterStimulationStart:
                    return Expression.Call(typeof(Wear), "ProcessRegisterStimulationStart", null, expression);

                /************************************************************************/
                /* Event: DEV_SELECT                                                    */
                /************************************************************************/
                case EventType.DeviceSelected:
                    return Expression.Call(typeof(Wear), "ProcessDeviceSelected", null, expression);
                case EventType.RegisterDeviceSelected:
                    return Expression.Call(typeof(Wear), "ProcessRegisterDeviceSelected", null, expression);

                /************************************************************************/
                /* Event: TEMP, BATTERY, TX_RETRIES and RX_GOOD                          */
                /************************************************************************/
                case EventType.SensorTemperature:
                    return Expression.Call(typeof(Wear), "ProcessSensorTemperature", null, expression);
                case EventType.Battery:
                    return Expression.Call(typeof(Wear), "ProcessBatery", null, expression);
                case EventType.TxRetries:
                    return Expression.Call(typeof(Wear), "ProcessTxRetries", null, expression);
                case EventType.RxGood:
                    return Expression.Call(typeof(Wear), "ProcessRxGood", null, expression);

                case EventType.RegisterSensorTemperature:
                    return Expression.Call(typeof(Wear), "ProcessRegisterSensorTemperature", null, expression);
                case EventType.RegisterBattery:
                    return Expression.Call(typeof(Wear), "ProcessRegisterBatery", null, expression);
                case EventType.RegisterTxRetries:
                    return Expression.Call(typeof(Wear), "ProcessRegisterTxRetries", null, expression);
                case EventType.RegisterRxGood:
                    return Expression.Call(typeof(Wear), "ProcessRegisterRxGood", null, expression);

                /************************************************************************/
                /* Event: CAM0 and CAM1                                                 */
                /************************************************************************/
                case EventType.RegisterCamera0:
                    return Expression.Call(typeof(Wear), "ProcessRegisterCamera0", null, expression);
                case EventType.RegisterCamera1:
                    return Expression.Call(typeof(Wear), "ProcessRegisterCamera1", null, expression);

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

        static bool is_evt33(HarpDataFrame input) { return ((input.Address == 33) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt34(HarpDataFrame input) { return ((input.Address == 34) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt35(HarpDataFrame input) { return ((input.Address == 35) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt36(HarpDataFrame input) { return ((input.Address == 36) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt37(HarpDataFrame input) { return ((input.Address == 37) && (input.Error == false) && (input.Id == MessageId.Event)); }

        static bool is_evt40(HarpDataFrame input) { return ((input.Address == 40) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt42(HarpDataFrame input) { return ((input.Address == 42) && (input.Error == false) && (input.Id == MessageId.Event)); }

        static bool is_evt43(HarpDataFrame input) { return ((input.Address == 43) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt44(HarpDataFrame input) { return ((input.Address == 44) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt45(HarpDataFrame input) { return ((input.Address == 45) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt55(HarpDataFrame input) { return ((input.Address == 55) && (input.Error == false) && (input.Id == MessageId.Event)); }

        /************************************************************************/
        /* Event: DATA                                                          */
        /************************************************************************/
        static IObservable<Mat> ProcessMotionAll(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input =>
            {
                var output = new Mat(9, 1, Depth.S16, 1);

                for (int i = 0; i < output.Rows; i++)
                    output.SetReal(i, BitConverter.ToInt16(input.Message, 11 + i * 2));

                return output;
            });
        }

        static IObservable<Mat> ProcessMotionAccelerometer(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input =>
            {
                var output = new Mat(3, 1, Depth.S16, 1);

                for (int i = 0; i < output.Rows; i++)
                    output.SetReal(i, BitConverter.ToInt16(input.Message, 11 + (i+0) * 2));

                return output;
            });
        }

        static IObservable<Mat> ProcessMotionGyroscope(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input =>
            {
                var output = new Mat(3, 1, Depth.S16, 1);

                for (int i = 0; i < output.Rows; i++)
                    output.SetReal(i, BitConverter.ToInt16(input.Message, 11 + (i+3) * 2));

                return output;
            });
        }

        static IObservable<Mat> ProcessMotionMagnetometer(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt34).Select(input =>
            {
                var output = new Mat(3, 1, Depth.S16, 1);

                for (int i = 0; i < output.Rows; i++)
                    output.SetReal(i, BitConverter.ToInt16(input.Message, 11 + (i + 6) * 2));

                return output;
            });
        }

        /************************************************************************/
        /* Event: MISC                                                          */
        /************************************************************************/
        static IObservable<int> ProcessAnalogInput(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt35).Select(input => { return (int)((UInt16)(BitConverter.ToUInt16(input.Message, 11) & (UInt16)(0x0FFF))); });
        }
        static IObservable<bool> ProcessDigitalInput0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt35).Select(input => { return ((input.Message[12] & (1 << 6)) == (1 << 6)); });
        }
        static IObservable<bool> ProcessDigitalInput1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt35).Select(input => { return ((input.Message[12] & (1 << 7)) == (1 << 7)); });
        }

        static IObservable<Timestamped<UInt16>> ProcessRegisterMisc(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt35).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: TEMP, BATTERY, TX_RETRIES and RX_GOOD                         */
        /************************************************************************/
        static IObservable<float> ProcessSensorTemperature(IObservable<HarpDataFrame> source)
        {            
            return source.Where(is_evt43).Select(input => { return ((float)(input.Message[11]) * 256) / 340 + 35; });
        }
        static IObservable<int> ProcessBatery(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt45).Select(input => { return (int)(input.Message[11]); });
        }
        static IObservable<int> ProcessTxRetries(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt44).Select(input => { return (int)BitConverter.ToUInt16(input.Message, 11); });
        }
        static IObservable<bool> ProcessRxGood(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt55).Select(input => { return (input.Message[11] == 1); });
        }

        static IObservable<Timestamped<byte>> ProcessRegisterSensorTemperature(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt43).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<byte>> ProcessRegisterBatery(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt45).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<UInt16>> ProcessRegisterTxRetries(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt44).Select(input => { return new Timestamped<UInt16>(BitConverter.ToUInt16(input.Message, 11), ParseTimestamp(input.Message, 5)); });
        }
        static IObservable<Timestamped<byte>> ProcessRegisterRxGood(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt55).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: ACQ_STATUS                                                    */
        /************************************************************************/
        static IObservable<bool> ProcessAcquiring(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt40).Select(input => { return ((input.Message[11] & 1) == 1); });
        }

        static IObservable<Timestamped<bool>> ProcessRegisterAcquisitionStatus(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt40).Select(input => { return new Timestamped<bool>(((input.Message[11] & 1) == 1), ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: DEV_SELECT                                                    */
        /************************************************************************/
        static IObservable<string> ProcessDeviceSelected(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt42).Select(input =>
            {
                switch (input.Message[11] & 3)
                {
                    case 0:
                        return "(0) Wired";
                    case 1:
                        return "(1) Wireless RF1";
                    case 2:
                        return "(2) Wireless RF2";
                    default:
                        return "";
                }
            });
        }

        static IObservable<Timestamped<byte>> ProcessRegisterDeviceSelected(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt42).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }

        /************************************************************************/
        /* Event: CAM0 and CAM1                                                 */
        /************************************************************************/
        static IObservable<Timestamped<byte>> ProcessRegisterCamera0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt36).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }

        static IObservable<Timestamped<byte>> ProcessRegisterCamera1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt37).Select(input => { return new Timestamped<byte>(input.Message[11], ParseTimestamp(input.Message, 5)); });
        }
    }
}
