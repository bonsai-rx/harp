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

namespace Bonsai.Harp
{
    public class HandleError : SelectBuilder
    {
        public HandleError()
        {
            IfError = ErrorChoice.DoNothing;
        }

        public ErrorChoice IfError { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (IfError)
            {
                case ErrorChoice.ThrowException:
                    return Expression.Call(typeof(HandleError), "ProcessThrowException", null, expression);
                case ErrorChoice.DoNothing:
                    return Expression.Call(typeof(HandleError), "ProcessDoNothing", null, expression);
                default:
                    throw new InvalidOperationException("Invalid choice.");
            }
        }

        static bool ProcessThrowException(HarpDataFrame input)
        {
            if (input.Error)
            {
                string payload;
                switch ((HarpTypes)(input.Message[4] & ~0x10))
                {
                    case HarpTypes.U8:
                        payload = ((byte)(input.Message[11])).ToString() + "(U8)";
                        break;
                    case HarpTypes.I8:
                        payload = ((sbyte)(input.Message[11])).ToString() + "(U8)";
                        break;
                    case HarpTypes.U16:
                        payload = (BitConverter.ToUInt16(input.Message, 11)).ToString() + "(U16)";
                        break;
                    case HarpTypes.I16:
                        payload = (BitConverter.ToInt16(input.Message, 11)).ToString() + "(I16)";
                        break;
                    case HarpTypes.U32:
                        payload = (BitConverter.ToUInt32(input.Message, 11)).ToString() + "(U32)";
                        break;
                    case HarpTypes.I32:
                        payload = (BitConverter.ToInt32(input.Message, 11)).ToString() + "(I32)";
                        break;
                    case HarpTypes.U64:
                        payload = (BitConverter.ToUInt64(input.Message, 11)).ToString() + "(U64)";
                        break;
                    case HarpTypes.I64:
                        payload = (BitConverter.ToInt64(input.Message, 11)).ToString() + "(I64)";
                        break;
                    case HarpTypes.Float:
                        payload = (BitConverter.ToSingle(input.Message, 11)).ToString() + "(Float)";
                        break;

                    default:
                        payload = "NaN";
                        break;
                }

                string exception;

                if (input.Id == MessageId.Write)
                    exception = "User tried an erroneous command: write value " + payload + " to address " + input.Address + ".";
                else
                    exception = "User tried an erroneous command: read from address " + input.Address + ".";

                throw new InvalidOperationException(exception);
            }
            else
                return false;
        }

        static bool ProcessDoNothing(HarpDataFrame input)
        {
            if (input.Error)
                return true;
            else
                return false;
        }
    }
}
