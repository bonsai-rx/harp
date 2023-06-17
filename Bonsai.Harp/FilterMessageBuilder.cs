﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to filter
    /// a sequence for Harp messages matching the specified register and message type.
    /// </summary>
    [DefaultProperty(nameof(Register))]
    [XmlType(Namespace = Constants.XmlNamespace)]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    [WorkflowElementIcon(typeof(ElementCategory), "Reactive.Condition")]
    public abstract class FilterMessageBuilder : HarpCombinatorBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 1, upperBound: 1);

        /// <inheritdoc/>
        public override Range<int> ArgumentRange => argumentRange;

        /// <summary>
        /// Gets or sets a value specifying the expected message type. This parameter is optional.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies the expected message type. This parameter is optional.")]
        public MessageType? MessageType { get; set; }

        /// <summary>
        /// Gets or sets the operator used to filter and select messages from
        /// specific device registers.
        /// </summary>
        [DesignOnly(true)]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The operator used to filter and select messages from specific device registers.")]
        [TypeConverter(typeof(CombinatorTypeConverter))]
        public object Register
        {
            get { return Operator; }
            set { Operator = value; }
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="MessageType"/> property
        /// should be serialized.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the <see cref="MessageType"/> should be serialized;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMessageType() => MessageType.HasValue;

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var register = Register;
            if (register == null)
            {
                throw new InvalidOperationException("A valid register operator object must be specified.");
            }

            var source = arguments.First();
            var registerType = register.GetType();
            var combinator = Expression.Constant(this, typeof(FilterMessageBuilder));
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
            return Expression.Call(combinator, nameof(Filter), null, source, argument);
        }

        IObservable<HarpMessage> Filter(IObservable<HarpMessage> source, int address)
        {
            var messageType = MessageType;
            if (messageType == null) return source.Where(address);
            else return source.Where(address, messageType.Value);
        }

        IObservable<HarpMessage> Filter(IGroupedObservable<int, HarpMessage> source, int address)
        {
            var messageType = MessageType;
            if (source.Key != address) return Observable.Empty<HarpMessage>();
            else if (messageType == null) return source;
            else return source.Where(messageType.Value);
        }

        IObservable<HarpMessage> Filter(IObservable<IGroupedObservable<int, HarpMessage>> source, int address)
        {
            var messageType = MessageType;
            return source.Where(group => group.Key == address)
                         .SelectMany(group => messageType != null ? group.Where(messageType.Value) : group);
        }

        IObservable<HarpMessage> Filter(IGroupedObservable<Type, HarpMessage> source, Type registerType)
        {
            var messageType = MessageType;
            if (source.Key != registerType) return Observable.Empty<HarpMessage>();
            else if (messageType == null) return source;
            else return source.Where(messageType.Value);
        }

        IObservable<HarpMessage> Filter(IObservable<IGroupedObservable<Type, HarpMessage>> source, Type registerType)
        {
            var messageType = MessageType;
            return source.Where(group => group.Key == registerType)
                         .SelectMany(group => messageType != null ? group.Where(messageType.Value) : group);
        }
    }
}
