using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.ComponentModel;

namespace Bonsai.Harp.Commands
{
    public enum PyControlCommandType : byte
    {
        UseBnc2,
        UseDio6B
    }

    [Description(
        "\n" +
        "UseBnc1: Any\n" +
        "UseDio6B: Any\n"
    )]
    public class PyControl : SelectBuilder, INamedElement
    {
        public PyControl()
        {
            Type = PyControlCommandType.UseBnc2;
        }

        string INamedElement.Name
        {
            get { return typeof(PyControl).Name + "." + Type.ToString(); }
        }

        public PyControlCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case PyControlCommandType.UseBnc2:
                    return Expression.Call(typeof(PyControl), "ProcessUseBnc1", new[] { expression.Type }, expression);
                case PyControlCommandType.UseDio6B:
                    return Expression.Call(typeof(PyControl), "ProcessUseDio6B", new[] { expression.Type }, expression);

                default:
                    break;
            }
            return expression;
        }

        static HarpDataFrame ProcessUseBnc2<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 42, 255, (byte)HarpType.U8, 1, 0));
        }

        static HarpDataFrame ProcessUseDio6B<TSource>(TSource input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 42, 255, (byte)HarpType.U8, 0, 0));
        }
    }
}
