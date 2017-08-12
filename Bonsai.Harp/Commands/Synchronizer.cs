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
    public enum SynchronizerCommandType : byte
    {
        Outputs
    }

    [Description(
        "\n" +
        "Outputs: Bitmask\n"
    )]

    public class Synchronizer : SelectBuilder, INamedElement
    {
        public Synchronizer()
        {
            Type = SynchronizerCommandType.Outputs;
        }

        string INamedElement.Name
        {
            get { return typeof(Synchronizer).Name + "." + Type.ToString(); }
        }

        public SynchronizerCommandType Type { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            switch (Type)
            {
                case SynchronizerCommandType.Outputs:
                    if (expression.Type != typeof(byte)) { expression = Expression.Convert(expression, typeof(byte)); }
                    return Expression.Call(typeof(Synchronizer), "ProcessOutputs", null, expression);

                default:
                    break;
            }
            return expression;
        }

        static HarpDataFrame ProcessOutputs(byte input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 33, 255, (byte)HarpType.U8, input, 0));
        }
    }
}
