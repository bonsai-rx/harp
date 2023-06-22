using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to filter
    /// and select messages from specific registers in a Harp device.
    /// </summary>
    [DefaultProperty(nameof(Register))]
    [XmlType(Namespace = Constants.XmlNamespace)]
    public abstract class ParseBuilder : RegisterCombinatorBuilder
    {
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

        internal override Expression BuildCombinator(Expression source, Expression argument)
        {
            var payload = Expression.Parameter(typeof(HarpMessage));
            var payloadSelector = Expression.Lambda(
                Expression.Call(Register.GetType(), nameof(HarpMessage.GetPayload), null, payload),
                payload);

            source = Expression.Call(typeof(ParseBuilder), nameof(Filter), null, source, argument);
            return Expression.Call(
                typeof(ParseBuilder),
                nameof(Process),
                new[] { payloadSelector.ReturnType },
                source,
                payloadSelector);
        }

        static IObservable<TResult> Process<TResult>(IObservable<HarpMessage> source, Func<HarpMessage, TResult> selector)
        {
            return source.Select(message =>
            {
                if (message.Error)
                {
                    throw new ArgumentException("Attempted to parse an error message.", nameof(message));
                }

                return selector(message);
            });
        }

        static IObservable<HarpMessage> Filter(IObservable<HarpMessage> source, int address)
        {
            return source.Where(message => message.Address == address);
        }

        static IObservable<HarpMessage> Filter(IGroupedObservable<int, HarpMessage> source, int address)
        {
            return source.Key == address ? source : Observable.Empty<HarpMessage>();
        }

        static IObservable<HarpMessage> Filter(IObservable<IGroupedObservable<int, HarpMessage>> source, int address)
        {
            return source.Where(group => group.Key == address).Merge();
        }

        static IObservable<HarpMessage> Filter(IGroupedObservable<Type, HarpMessage> source, Type registerType)
        {
            return source.Key == registerType ? source : Observable.Empty<HarpMessage>();
        }

        static IObservable<HarpMessage> Filter(IObservable<IGroupedObservable<Type, HarpMessage>> source, Type registerType)
        {
            return source.Where(group => group.Key == registerType).Merge();
        }
    }
}
