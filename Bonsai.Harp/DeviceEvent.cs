using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.ComponentModel;

/* Events are divided into two categories: Bonsai Events and Raw Registers.                                                                                       */
/*   - Bonsai Events:                                                                                                                                             */
/*                   Should follow Bonsai guidelines and use the types int, bool, float, Mat and string (for Enums like Wear's DEV_SELECT                         */
/*   - Raw Registers:                                                                                                                                             */
/*                   Should have the Timestamped<T> output and the T must have the exact same type (UInt16, Int16, byte, Int, ...) of the Harp device register.   */
/*                                                                                                                                                                */
/* Note: When the device has both digital and analog inputs or outputs use the names DigitalOutput, DigitalInput, AnalogOutput and AnalogInput.                   */

/* Example of Events' descriptions (Bonsai Events):     */
/*      Bitmask (not recommended, use Mat[] instead)    */
/*      Groumask                                        */
/*      Boolean Mat[9]                                  */
/*      Boolean                                         */
/*      Boolean (*)                                     */
/*      Integer                                         */
/*      Integer Mat[3][9]                               */
/*      Decimal                                         */
/*      Decimal (V)                                     */
/*      Decimal (ºC)                                    */
/*  (*) Only distinct contiguous elements are propagated. */

/* Example of Events' descriptions (Raw Registers):     */
/*      Bitmask U8                                      */
/*      Groupmask U8                                    */
/*      U16                                             */
/*      S16                                             */
/*      U32                                             */
/*      Float                                           */
/*      INPUTS register U16                             */



namespace Bonsai.Harp
{
    [Description(
    "\n" +
    "Heartbeat: Integer\n" +
    "\n" +
    "MessageTimestamp: Double\n"
    )]
    public class DeviceEvent : SingleArgumentExpressionBuilder, INamedElement
    {
        string INamedElement.Name => $"Device.{Type}";

        public DeviceEventType Type { get; set; } = DeviceEventType.Heartbeat;

        public override Expression Build(IEnumerable<Expression> expressions)
        {
            var expression = expressions.First();
            switch (Type)
            {
                case DeviceEventType.Heartbeat:
                    return Expression.Call(typeof(DeviceEvent), nameof(Heartbeat), null, expression);
                case DeviceEventType.MessageTimestamp:
                    return Expression.Call(typeof(DeviceEvent), nameof(MessageTimestamp), null, expression);
                default:
                    throw new InvalidOperationException("Invalid selection or not supported yet.");
            }
        }

        static IObservable<uint> Heartbeat(IObservable<HarpMessage> source)
        {
            return source
                .Where(input => input.IsMatch(Registers.TimestampSecond, MessageType.Event, PayloadType.TimestampedU32))
                .Select(input => input.GetPayloadUInt32());
        }

        static IObservable<double> MessageTimestamp(IObservable<HarpMessage> source)
        {
            return source
                .Where(input => input.MessageType == MessageType.Event && input.IsTimestamped)
                .Select(input => input.GetTimestamp());
        }
    }

    public enum DeviceEventType : byte
    {
        Heartbeat = 0,
        MessageTimestamp
    }
}
