using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to filter
    /// and select messages from specific registers in a Harp device.
    /// </summary>
    [DefaultProperty(nameof(Register))]
    public abstract class ParseBuilder : HarpCombinatorBuilder
    {
        static readonly Range<int> argumentRange = Range.Create(lowerBound: 1, upperBound: 1);

        /// <inheritdoc/>
        public override Range<int> ArgumentRange => argumentRange;

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
            get { return Combinator; }
            set { Combinator = value; }
        }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            return base.Build(arguments
                .Where(expression => expression.Type.GenericTypeArguments[0] == typeof(HarpMessage)));
        }
    }
}
