using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Xml.Serialization;
using Bonsai.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to filter
    /// and select messages from specific registers in a Harp device.
    /// </summary>
    [XmlType(Namespace = Constants.XmlNamespace)]
    public abstract class ParseBuilder : FilterMessageBuilder
    {
        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var register = Register;
            var source = base.Build(arguments);
            var registerType = register.GetType();
            var payload = Expression.Parameter(typeof(HarpMessage));
            var payloadSelector = Expression.Lambda(
                Expression.Call(registerType, nameof(HarpMessage.GetPayload), null, payload),
                payload);
            return Expression.Call(
                typeof(Observable),
                nameof(Observable.Select),
                new[] { payload.Type, payloadSelector.ReturnType },
                source,
                payloadSelector);
        }
    }
}
