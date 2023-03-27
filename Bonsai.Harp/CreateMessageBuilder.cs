using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml.Serialization;
using Bonsai.Expressions;

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
        readonly CombinatorBuilder builder = new CombinatorBuilder();

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
