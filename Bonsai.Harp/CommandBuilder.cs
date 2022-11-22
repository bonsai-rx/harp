using System.ComponentModel;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to create
    /// command messages for Harp devices.
    /// </summary>
    [DefaultProperty(nameof(Command))]
    public abstract class CommandBuilder : HarpCombinatorBuilder
    {
        /// <summary>
        /// Gets or sets the type of the device command message to create.
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
            get { return Combinator; }
            set { Combinator = value; }
        }
    }
}
