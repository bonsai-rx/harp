using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Bonsai.Harp.Commands
{
    public enum WearCommandType : byte
    {
        Start,
        Stop,
        WriteLed,
        WritePosition,
        WriteAnalog
    }

    public class Wear : SelectBuilder, INamedElement
    {
        public Wear()
        {
            Type = WearCommandType.Start;
        }

        string INamedElement.Name
        {
            get { return typeof(Wear).Name + "." + Type.ToString(); }
        }

        public WearCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case WearCommandType.Start:
                    return Expression.Call(typeof(Wear), "ProcessStart", new[] { expression.Type }, expression);
                case WearCommandType.Stop:
                    return Expression.Call(typeof(Wear), "ProcessStop", new[] { expression.Type }, expression);
                case WearCommandType.WriteLed:
                    if (expression.Type != typeof(byte))
                    {
                        expression = Expression.Convert(expression, typeof(byte));
                    }
                    return Expression.Call(typeof(Wear), "ProcessWriteLed", null, expression);
                case WearCommandType.WritePosition:
                    break;
                case WearCommandType.WriteAnalog:
                    break;
                default:
                    break;
            }
            return expression;
        }

        static HarpDataFrame ProcessStart<TSource>(TSource input)
        {
            return new HarpDataFrame(0x1, 0x2, 0x3, 0x5);
        }

        static HarpDataFrame ProcessWriteLed(byte input)
        {
            return new HarpDataFrame(0x1, 0x2, 0x3, input);
        }

        static HarpDataFrame ProcessWritePosition(int input)
        {
            return new HarpDataFrame(0x1, 0x2, 0x3, (byte)(input>>24),(byte)(input>>16),(byte)(input>>8),(byte)input);
        }
    }
}
