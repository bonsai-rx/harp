using System.ComponentModel;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for operators used to manipulate
    /// Harp device messages.
    /// </summary>
    [Combinator]
    [DesignTimeVisible(false)]
    [XmlType(Namespace = Constants.XmlNamespace)]
    public abstract class HarpCombinator
    {
        /// <summary>
        /// Gets or sets a value specifying the type of Harp device message to process.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies the type of Harp device message to process.")]
        public MessageType MessageType { get; set; } = MessageType.Write;
    }
}
