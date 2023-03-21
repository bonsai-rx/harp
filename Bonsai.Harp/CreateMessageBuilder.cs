using System.ComponentModel;
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
            get { return Combinator; }
            set { Combinator = value; }
        }
    }
}
