using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Xml.Serialization;
using Bonsai.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to format
    /// a sequence of values as specific Harp device register messages.
    /// </summary>
    [DefaultProperty(nameof(Register))]
    [XmlType(Namespace = Constants.XmlNamespace)]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public abstract class FormatBuilder : HarpCombinatorBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 1, upperBound: 1);

        /// <inheritdoc/>
        public override Range<int> ArgumentRange => argumentRange;

        /// <summary>
        /// Gets or sets a value specifying the type of the formatted message.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies the type of the formatted message.")]
        public MessageType MessageType { get; set; } = MessageType.Write;

        /// <summary>
        /// Gets or sets the operator used to format the source data into specific
        /// Harp device register messages.
        /// </summary>
        [DesignOnly(true)]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The operator used to format the source data into specific Harp device register messages.")]
        [TypeConverter(typeof(CombinatorTypeConverter))]
        public object Register
        {
            get { return Operator; }
            set { Operator = value; }
        }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var register = Register;
            if (register == null)
            {
                throw new InvalidOperationException("A valid register operator object must be specified.");
            }

            if (register is ExpressionBuilder builder)
            {
                return builder.Build(arguments);
            }

            var source = arguments.First();
            var registerType = register.GetType();
            var payloadType = source.Type.GenericTypeArguments[0];

            ParameterExpression[] selectorParameters;
            var messageType = Expression.Parameter(typeof(MessageType), "messageType");
            if (payloadType.IsGenericType && payloadType.GetGenericTypeDefinition() == typeof(Timestamped<>))
            {
                payloadType = payloadType.GenericTypeArguments[0];
                var timestamp = Expression.Parameter(typeof(double), "timestamp");
                var value = Expression.Parameter(payloadType, "value");
                selectorParameters = new[] { timestamp, messageType, value };
            }
            else
            {
                var value = Expression.Parameter(payloadType, "value");
                selectorParameters = new[] { messageType, value };
            }

            var selectorMethod = registerType.GetMethods().FirstOrDefault(m =>
                m.Name == nameof(HarpMessage.FromPayload) &&
                m.GetParameters().Length == selectorParameters.Length);
            if (selectorMethod == null)
            {
                throw new InvalidOperationException(
                    $"No compatible method '{nameof(HarpMessage.FromPayload)}' was found for the specified register.");
            }

            var parameterIndex = 0;
            var methodParameters = selectorMethod.GetParameters();
            var selectorArguments = Array.ConvertAll(selectorMethod.GetParameters(), parameter =>
            {
                var selectorParameter = selectorParameters[parameterIndex++];
                return parameter.ParameterType != selectorParameter.Type
                    ? (Expression)Expression.Convert(selectorParameter, parameter.ParameterType)
                    : selectorParameter;
            });

            var combinator = Expression.Constant(this, typeof(FormatBuilder));
            var selector = Expression.Lambda(
                Expression.Call(registerType, nameof(HarpMessage.FromPayload), null, selectorArguments),
                selectorParameters);
            return Expression.Call(
                combinator,
                nameof(Process),
                new[] { payloadType },
                source,
                selector);
        }

        IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source, Func<MessageType, TSource, HarpMessage> selector)
        {
            var messageType = MessageType;
            return source.Select(payload => selector(messageType, payload));
        }

        IObservable<HarpMessage> Process<TSource>(IObservable<Timestamped<TSource>> source, Func<double, MessageType, TSource, HarpMessage> selector)
        {
            var messageType = MessageType;
            return source.Select(payload => selector(payload.Seconds, messageType, payload.Value));
        }
    }
}
