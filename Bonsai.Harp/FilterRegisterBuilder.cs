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
    /// a sequence of Harp messages matching the specified register.
    /// </summary>
    [DefaultProperty(nameof(Register))]
    [XmlType(Namespace = Constants.XmlNamespace)]
    [WorkflowElementIcon(typeof(ElementCategory), "Reactive.Condition")]
    public abstract class FilterRegisterBuilder : RegisterCombinatorBuilder
    {
        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies how the message filter will use the matching criteria.")]
        public FilterType FilterType { get; set; }

        /// <summary>
        /// Gets or sets the operator used to filter messages from specific device registers.
        /// </summary>
        [DesignOnly(true)]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The operator used to filter messages from specific device registers.")]
        [TypeConverter(typeof(CombinatorTypeConverter))]
        public object Register
        {
            get { return Operator; }
            set { Operator = value; }
        }

        internal override Expression BuildCombinator(Expression source, Expression argument)
        {
            var combinator = Expression.Constant(this, typeof(FilterRegisterBuilder));
            return Expression.Call(combinator, nameof(Filter), null, source, argument);
        }

        IObservable<HarpMessage> Filter(IObservable<HarpMessage> source, int address)
        {
            return FilterType == FilterType.Include
                ? source.Where(message => message.Address == address)
                : source.Where(message => message.Address != address);
        }

        IObservable<HarpMessage> Filter(IGroupedObservable<int, HarpMessage> source, int address)
        {
            var match = source.Key == address;
            var includeMatch = FilterType == FilterType.Include;
            return match == includeMatch ? source : Observable.Empty<HarpMessage>();
        }

        IObservable<HarpMessage> Filter(IObservable<IGroupedObservable<int, HarpMessage>> source, int address)
        {
            return (FilterType == FilterType.Include
                ? source.Where(group => group.Key == address)
                : source.Where(group => group.Key != address))
                .Merge();
        }

        IObservable<HarpMessage> Filter(IGroupedObservable<Type, HarpMessage> source, Type registerType)
        {
            var match = source.Key == registerType;
            var includeMatch = FilterType == FilterType.Include;
            return match == includeMatch ? source : Observable.Empty<HarpMessage>();
        }

        IObservable<HarpMessage> Filter(IObservable<IGroupedObservable<Type, HarpMessage>> source, Type registerType)
        {
            return (FilterType == FilterType.Include
                ? source.Where(group => group.Key == registerType)
                : source.Where(group => group.Key != registerType))
                .Merge();
        }
    }
}
