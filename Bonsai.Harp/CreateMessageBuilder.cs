using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to create
    /// specific Harp device message payloads.
    /// </summary>
    [DefaultProperty(nameof(Payload))]
    [XmlType(Namespace = Constants.XmlNamespace)]
    [WorkflowElementCategory(ElementCategory.Source)]
    public abstract class CreateMessageBuilder : HarpCombinatorBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 0, upperBound: 1);

        /// <inheritdoc/>
        public override Range<int> ArgumentRange => argumentRange;

        /// <summary>
        /// Gets or sets a value specifying the type of the created message.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies the type of the created message.")]
        public MessageType MessageType { get; set; } = MessageType.Write;

        /// <summary>
        /// Gets or sets the operator used to create specific Harp device message payloads.
        /// </summary>
        [DesignOnly(true)]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The operator used to create specific Harp device message payloads.")]
        [TypeConverter(typeof(CombinatorTypeConverter))]
        public object Payload
        {
            get { return Operator; }
            set { Operator = value; }
        }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var source = arguments.FirstOrDefault();
            var payload = Expression.Constant(Payload);
            var messageType = Expression.Parameter(typeof(MessageType));
            var combinator = Expression.Constant(this, typeof(CreateMessageBuilder));

            var timestamped = false;
            MethodInfo getMessage = null;
            var getMessageOverloads = payload.Type.FindMembers(
                MemberTypes.Method,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                Type.FilterName,
                nameof(CreateMessagePayload.GetMessage));
            for (int i = 0; i < getMessageOverloads.Length; i++)
            {
                var method = (MethodInfo)getMessageOverloads[i];
                var methodParameters = method.GetParameters();
                if (methodParameters.Length == 0 || methodParameters.Length > 2 ||
                    methodParameters[methodParameters.Length - 1].ParameterType != typeof(MessageType))
                {
                    continue;
                }

                if (methodParameters.Length == 1 && getMessage == null) getMessage = method;
                else if (methodParameters[0].ParameterType == typeof(double) &&
                    (payload.Value is not CreateMessagePayload createMessage ||
                    (createMessage.PayloadType & PayloadType.Timestamp) != 0))
                {
                    timestamped = true;
                    getMessage = method;
                }
            }

            if (getMessage == null)
            {
                throw new InvalidOperationException("No compatible method overload found for the given arguments.");
            }

            if (timestamped)
            {
                if (source == null)
                {
                    throw new InvalidOperationException("Creating timestamped messages requires a source input.");
                }

                var timestamp = Expression.Parameter(typeof(double));
                var selector = Expression.Lambda(
                    Expression.Call(payload, getMessage, timestamp, messageType),
                    timestamp,
                    messageType);
                var sourceType = source.Type.GetGenericArguments()[0];
                var typeArguments = sourceType != typeof(HarpMessage) && sourceType.IsGenericType
                    ? new[] { sourceType.GetGenericArguments()[0] }
                    : null;
                return Expression.Call(
                    combinator,
                    nameof(ProcessTimestamped),
                    typeArguments,
                    source,
                    selector);
            }
            else
            {
                var selector = Expression.Lambda(
                    Expression.Call(payload, getMessage, messageType),
                    messageType);
                if (source == null)
                {
                    return Expression.Call(combinator, nameof(Process), null, selector);
                }
                else
                {
                    var sourceType = source.Type.GetGenericArguments()[0];
                    return Expression.Call(
                        combinator,
                        nameof(Process),
                        new[] { sourceType },
                        source,
                        selector);
                }
            }
        }

        IObservable<HarpMessage> Process(Func<MessageType, HarpMessage> selector)
        {
            return Observable.Defer(() => Observable.Return(selector(MessageType)));
        }

        IObservable<HarpMessage> Process<TSource>(IObservable<TSource> source, Func<MessageType, HarpMessage> selector)
        {
            return source.Select(_ => selector(MessageType));
        }

        IObservable<HarpMessage> ProcessTimestamped(IObservable<HarpMessage> source, Func<double, MessageType, HarpMessage> selector)
        {
            return source.Select(message => selector(message.GetTimestamp(), MessageType));
        }

        IObservable<HarpMessage> ProcessTimestamped<TSource>(IObservable<Timestamped<TSource>> source, Func<double, MessageType, HarpMessage> selector)
        {
            return source.Select(_ => selector(_.Seconds, MessageType));
        }
    }
}
