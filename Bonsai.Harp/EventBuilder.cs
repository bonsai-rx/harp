using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to parse
    /// event messages for Harp devices.
    /// </summary>
    [DefaultProperty(nameof(Event))]
    public abstract class EventBuilder : HarpCombinatorBuilder
    {
        /// <summary>
        /// Gets or sets the type of the device event message to select.
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
