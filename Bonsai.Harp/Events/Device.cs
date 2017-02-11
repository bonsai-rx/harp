﻿using OpenCV.Net;
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
    public class Device : SingleArgumentExpressionBuilder, INamedElement
    {
        public Device()
        {
            Type = EventType.Timestamp;
        }

        string INamedElement.Name
        {
            get { return typeof(Device).Name + "." + Type.ToString(); }
        }

        public enum EventType : byte
        {
            /* Event: TIMESTAMP_SECOND */
            Timestamp = 0,
            TimestampRegister,
        }

        public EventType Type { get; set; }

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                /************************************************************************/
                /* List of Events                                                       */
                /************************************************************************/
                case EventType.Timestamp:
                    return Expression.Call(typeof(Device), "ProcessEVT_Timestamp", null, expression);
                case EventType.TimestampRegister:
                    return Expression.Call(typeof(Device), "ProcessEVT_TimestampRaw", null, expression);

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

        static bool is_evt_timestamp(HarpDataFrame input) { return ((input.Address == 8) && (input.Error == false) && (input.Id == MessageId.Event)); }

        /************************************************************************/
        /* Process Events                                                       */
        /************************************************************************/
        static IObservable<UInt32> ProcessEVT_Timestamp(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt_timestamp).Select(input =>
            {
                return BitConverter.ToUInt32(input.Message, 11);
            });
        }

        static IObservable<Timestamped<UInt32>> ProcessEVT_TimestampRaw(IObservable<HarpDataFrame> source)
        {
            return source.Where(is_evt_timestamp).Select(input =>
            {
                var timestamp = ParseTimestamp(input.Message, 5);
                var value = BitConverter.ToUInt32(input.Message, 11);
                return new Timestamped<UInt32>(value, timestamp);
            });
        }        
    }
}