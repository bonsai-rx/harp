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
    public enum SynchronizerCommandType : byte
    {
        WriteOutput0
    }

    public class Synchronizer : SelectBuilder, INamedElement
    {
        public Synchronizer()
        {
            Type = SynchronizerCommandType.WriteOutput0;
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
                case SynchronizerCommandType.WriteOutput0:
                    if (expression.Type != typeof(bool))
                    {
                        expression = Expression.Convert(expression, typeof(bool));
                    }
                    return Expression.Call(typeof(Synchronizer), "ProcessWriteOutput0", null, expression);

                default:
                    break;
            }
            return expression;
        }

        static HarpDataFrame ProcessWriteOutput0(bool input)
        {
            return HarpDataFrame.UpdateChesksum(new HarpDataFrame(2, 5, 33, 255, (byte)HarpType.U8, (byte)(input ? 1 : 0), 0));
        }
    }
}
