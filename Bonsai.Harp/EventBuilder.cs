using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to filter
    /// and parse event messages from Harp devices.
    /// </summary>
    [DefaultProperty(nameof(Event))]
    public abstract class EventBuilder : HarpCombinatorBuilder, INamedElement
    {
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
            get { return Combinator; }
            set { Combinator = value; }
        }
    }
}
