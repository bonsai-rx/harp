using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Bonsai.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to filter
    /// and parse event messages from Harp devices.
    /// </summary>
    [Obsolete]
    [DefaultProperty(nameof(Event))]
    public abstract class EventBuilder : HarpCombinatorBuilder, INamedElement
    {
        readonly CombinatorBuilder builder = new CombinatorBuilder();

        string INamedElement.Name => $"{RemoveSuffix(GetType().Name, nameof(Event))}.{GetElementDisplayName(Event)}";

        /// <summary>
        /// Gets or sets the event parser used to filter and select event messages
        /// reported by the device.
        /// </summary>
        [DesignOnly(true)]
        [DisplayName("Type")]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The type of the device event message to select.")]
        [TypeConverter(typeof(CombinatorTypeConverter))]
        public object Event
        {
            get { return Operator; }
            set { builder.Combinator = Operator = value; }
        }

        /// <inheritdoc/>
        public override Range<int> ArgumentRange => builder.ArgumentRange;

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            return builder.Build(arguments);
        }
    }
}
