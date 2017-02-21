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

            /* Event: BATTERY, TX_RETRIES and RX_GOOD */
            SensorTemperature,
            Battery,
            TxRetries,
            RxGood,

            /* Event: MISC */
            RegisterAnalogInput,
            RegisterDigitalInput0,
            RegisterDigitalInput1,

            /* Event: ACQ_STATUS */
            RegisterAcquiring,

            /* Event: START_STIM */
            RegisterStimulationStart,

            /* Event: DEV_SELECT */
            RegisterDeviceSelected,

            /* Event: CAM0 and CAM1 */
            RegisterCamera0,
            RegisterCamera1,

            /* Event: BATTERY, TX_RETRIES and RX_GOOD */
            RegisterSensorTemperature,
            RegisterBattery,
            RegisterTxRetries,
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
                    return Expression.Call(typeof(Wear), "ProcessMotionMotionMagnetometer", null, expression);
                
                /************************************************************************/
                /* Event: MISC                                                          */
                /************************************************************************/
                case EventType.AnalogInput:
                    return Expression.Call(typeof(Wear), "ProcessAnalogInput", null, expression);
                case EventType.DigitalInput0:
                    return Expression.Call(typeof(Wear), "ProcessDigitalInput0", null, expression);
                case EventType.DigitalInput1:
                    return Expression.Call(typeof(Wear), "ProcessDigitalInput1", null, expression);


                case EventType.RegisterAnalogInput:
                    return Expression.Call(typeof(Wear), "ProcessRegisterAnalogInput", null, expression);
                case EventType.RegisterDigitalInput0:
                    return Expression.Call(typeof(Wear), "ProcessRegisterDigitalInput0", null, expression);
                case EventType.RegisterDigitalInput1:
                    return Expression.Call(typeof(Wear), "ProcessRegisterDigitalInput1", null, expression);

                /************************************************************************/
                /* Event: ACQ_STATUS                                                    */
                /************************************************************************/
                case EventType.Acquiring:
                    return Expression.Call(typeof(Wear), "ProcessAcquiring", null, expression);
                case EventType.RegisterAcquiring:
                    return Expression.Call(typeof(Wear), "ProcessRegisterAcquiring", null, expression);

                /************************************************************************/
                /* Event: START_STIM                                                    */
                /************************************************************************/
                case EventType.RegisterStimulationStart:
                    return Expression.Call(typeof(Wear), "ProcessRegisterStimulationStart", null, expression);

                /************************************************************************/
                /* Event: DEV_SELECT                                                    */
                /************************************************************************/
                case EventType.RegisterDeviceSelected:
                    return Expression.Call(typeof(Wear), "ProcessRegisterDeviceSelected", null, expression);

                /************************************************************************/
                /* Event: BATTERY, TX_RETRIES and RX_GOOD                               */
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

        /************************************************************************/
        /* Event: ACQ_STATUS                                                    */
        /************************************************************************/
        static IObservable<bool> ProcessAcquiring(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt40).Select(input =>
            {
                return ((input.Message[11] & 1) == 1);
            });
        }

        static IObservable<Timestamped<bool>> ProcessRegisterAcquiring(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt40).Select(input =>
            {
                var timestamp = ParseTimestamp(input.Message, 5);

                if ((input.Message[11] & 1) == 1)
                    return new Timestamped<bool>(true, timestamp);
                else
                    return new Timestamped<bool>(false, timestamp);
            });
        }

        /************************************************************************/
        /* Event: DEV_SELECT                                                    */
        /************************************************************************/
        static IObservable<Timestamped<string>> ProcessRegisterDeviceSelected(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt42).Select(input =>
            {
                var timestamp = ParseTimestamp(input.Message, 5);
                string MyOutput;

                switch (input.Message[11] & 3)
                {
                    case 0: MyOutput = "Wired";
                        break;
                    case 1: MyOutput = "Wireless (RF1)";
                        break;
                    case 2: MyOutput = "Wireless (RF2)";
                        break;
                    default: MyOutput = "";
                        break;
                }

                return new Timestamped<string>(MyOutput, timestamp);
            });
        }

        /************************************************************************/
        /* Event: CAM0 and CAM1                                                 */
        /************************************************************************/
        static IObservable<Timestamped<string>> ProcessRegisterCamera0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt36).Select(input =>
            {
                var timestamp = ParseTimestamp(input.Message, 5);
                return new Timestamped<string>("Camera0", timestamp);
            });
        }

        static IObservable<Timestamped<string>> ProcessRegisterCamera1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt37).Select(input =>
            {
                var timestamp = ParseTimestamp(input.Message, 5);
                return new Timestamped<string>("Camera1", timestamp);
            });
        }
    }
}
