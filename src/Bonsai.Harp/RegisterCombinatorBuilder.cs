using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to filter
    /// and select a sequence of Harp messages matching the specified register.
    /// </summary>
    [XmlType(Namespace = Constants.XmlNamespace)]
    public abstract class RegisterCombinatorBuilder : HarpCombinatorBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 1, upperBound: 1);

        /// <inheritdoc/>
        public override Range<int> ArgumentRange => argumentRange;

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var register = Operator;
            if (register == null)
            {
                throw new InvalidOperationException("A valid register operator object must be specified.");
            }

            var source = arguments.First();
            var registerType = register.GetType();
            var parameterType = source.Type.GetGenericArguments()[0];
            var argument = (Expression)Expression.Field(null, registerType, nameof(HarpMessage.Address));
            if (parameterType.IsGenericType &&
                parameterType.GetGenericTypeDefinition() == typeof(IGroupedObservable<,>))
            {
                var keyType = parameterType.GetGenericArguments()[0];
                if (keyType == typeof(Type))
                {
                    argument = Expression.Constant(registerType);
                }
            }

            return BuildCombinator(source, argument);
        }

        internal abstract Expression BuildCombinator(Expression source, Expression argument);
    }
}
