using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Bonsai.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to create
    /// command messages for Harp devices.
    /// </summary>
    [Obsolete]
    [DefaultProperty(nameof(Command))]
    public abstract class CommandBuilder : HarpCombinatorBuilder, INamedElement
    {
        readonly CombinatorBuilder builder = new CombinatorBuilder();

        string INamedElement.Name => $"{RemoveSuffix(GetType().Name, nameof(Command))}.{GetElementDisplayName(Command)}";

        /// <summary>
        /// Gets or sets the command formatter used to create command messages.
        /// </summary>
        [DesignOnly(true)]
        [DisplayName("Type")]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The type of the device command message to create.")]
        [TypeConverter(typeof(CombinatorTypeConverter))]
        public object Command
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
