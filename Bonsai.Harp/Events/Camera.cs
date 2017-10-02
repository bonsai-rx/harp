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
    public enum CameraEventType : byte
    {
        /* Event: INPUT0 */
        Input0,

        /* Events: CAM0, CAM1, SYNC0 and SYNC1 */
        Trig0,
        Sync0,
        Trig1,
        Sync1
    }

    [Description(
        "\n" +
        "Input0: Boolean\n" +
        "\n" +
        "Trig0: Boolean\n" +
        "Sync0: Boolean\n" +
        "\n" +
        "Trig1: Boolean\n" +
        "Sync1: Boolean\n"
    )]

    public class Camera : SingleArgumentExpressionBuilder, INamedElement
    {
        public Camera()
        {
            Type = CameraEventType.Input0;
        }

        string INamedElement.Name
        {
            get { return typeof(Camera).Name + "." + Type.ToString(); }
        }

        public CameraEventType Type { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* Register: INPUT0                                                     */
                /************************************************************************/
                case CameraEventType.Input0:
                    return Expression.Call(typeof(Camera), "ProcessInputs0", null, expression);

                /************************************************************************/
                /* Register:  CAM0, CAM1, SYNC0 and SYNC1                               */
                /************************************************************************/
                case CameraEventType.Trig0:
                    return Expression.Call(typeof(Camera), "ProcessTrig0", null, expression);
                case CameraEventType.Sync0:
                    return Expression.Call(typeof(Camera), "ProcessSync0", null, expression);
                case CameraEventType.Trig1:
                    return Expression.Call(typeof(Camera), "ProcessTrig1", null, expression);
                case CameraEventType.Sync1:
                    return Expression.Call(typeof(Camera), "ProcessSync1", null, expression);

                /************************************************************************/
                /* Default                                                              */
                /************************************************************************/
                default:
                    throw new InvalidOperationException("Invalid selection or not supported yet.");
            }
        }

        static bool is_evt39(HarpDataFrame input) { return ((input.Address == 39) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt40(HarpDataFrame input) { return ((input.Address == 40) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt41(HarpDataFrame input) { return ((input.Address == 41) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt42(HarpDataFrame input) { return ((input.Address == 42) && (input.Error == false) && (input.Id == MessageId.Event)); }
        static bool is_evt43(HarpDataFrame input) { return ((input.Address == 43) && (input.Error == false) && (input.Id == MessageId.Event)); }
                
        /************************************************************************/
        /* Register: INPUT0                                                     */
        /************************************************************************/
        static IObservable<bool> ProcessInputs0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt39).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)) ? true : false; });
        }

        /************************************************************************/
        /* Register: CAM0, CAM1, SYNC0 and SYNC1                                */
        /************************************************************************/
        static IObservable<bool> ProcessTrig0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt40).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)) ? true : false; });
        }
        static IObservable<bool> ProcessSync0(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt42).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)) ? true : false; });
        }
        static IObservable<bool> ProcessTrig1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt41).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)) ? true : false; });
        }
        static IObservable<bool> ProcessSync1(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt43).Select(input => { return ((input.Message[11] & (1 << 0)) == (1 << 0)) ? true : false; });
        }
    }
}
