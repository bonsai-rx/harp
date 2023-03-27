using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that filters the sequence for Harp messages matching
    /// the specified register and message type.
    /// </summary>
    /// <seealso cref="FilterMessageAddress"/>
    /// <seealso cref="WhoAmI"/>
    /// <seealso cref="HardwareVersionHigh"/>
    /// <seealso cref="HardwareVersionLow"/>
    /// <seealso cref="AssemblyVersion"/>
    /// <seealso cref="CoreVersionHigh"/>
    /// <seealso cref="CoreVersionLow"/>
    /// <seealso cref="FirmwareVersionHigh"/>
    /// <seealso cref="FirmwareVersionLow"/>
    /// <seealso cref="TimestampSeconds"/>
    /// <seealso cref="TimestampMicroseconds"/>
    /// <seealso cref="OperationControl"/>
    /// <seealso cref="ResetDevice"/>
    /// <seealso cref="DeviceName"/>
    /// <seealso cref="SerialNumber"/>
    /// <seealso cref="ClockConfiguration"/>
    [XmlInclude(typeof(FilterMessageAddress))]
    [XmlInclude(typeof(WhoAmI))]
    [XmlInclude(typeof(HardwareVersionHigh))]
    [XmlInclude(typeof(HardwareVersionLow))]
    [XmlInclude(typeof(AssemblyVersion))]
    [XmlInclude(typeof(CoreVersionHigh))]
    [XmlInclude(typeof(CoreVersionLow))]
    [XmlInclude(typeof(FirmwareVersionHigh))]
    [XmlInclude(typeof(FirmwareVersionLow))]
    [XmlInclude(typeof(TimestampSeconds))]
    [XmlInclude(typeof(TimestampMicroseconds))]
    [XmlInclude(typeof(OperationControl))]
    [XmlInclude(typeof(ResetDevice))]
    [XmlInclude(typeof(DeviceName))]
    [XmlInclude(typeof(SerialNumber))]
    [XmlInclude(typeof(ClockConfiguration))]
    [Description("Filters the sequence for Harp messages that match the specified register and message type.")]
    [WorkflowElementIcon(typeof(ElementCategory), "Reactive.Condition")]
    public class FilterMessage : FilterMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMessage"/> class.
        /// </summary>
        public FilterMessage()
        {
            Register = new FilterMessageAddress();
        }

        string INamedElement.Name => Register is FilterMessageAddress
            ? default
            : $"Device.{GetElementDisplayName(Register)}";

        /// <summary>
        /// Gets or sets a value specifying the expected message type. This parameter is optional.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies the expected message type. This parameter is optional.")]
        public new MessageType? MessageType
        {
            get { return base.MessageType; }
            set
            {
                base.MessageType = value;
                if (Register is FilterMessageAddress filterMessage)
                {
                    filterMessage.MessageType = value;
                }
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? Address
        {
            get { return Register is FilterMessageAddress filterMessage ? filterMessage.Address : default; }
            set { if (Register is FilterMessageAddress filterMessage) filterMessage.Address = value; }
        }

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeAddress() => false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            if (Register is FilterMessageAddress filterMessage)
            {
                var source = arguments.First();
                var combinator = Expression.Constant(filterMessage);
                return Expression.Call(combinator, nameof(FilterMessageAddress.Process), null, source);
            }
            else return base.Build(arguments);
        }
    }

    /// <summary>
    /// Represents an operator that filters the sequence for Harp messages matching
    /// the specified address and message type.
    /// </summary>
    [DesignTimeVisible(false)]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    [Description("Filters the sequence for Harp messages matching the specified address and message type.")]
    public class FilterMessageAddress : Combinator<HarpMessage, HarpMessage>
    {
        /// <summary>
        /// Gets or sets a value specifying the expected message type. This parameter is optional.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        [Description("Specifies the expected message type. This parameter is optional.")]
        public MessageType? MessageType { get; set; }

        /// <summary>
        /// Gets or sets the expected message address. This parameter is optional.
        /// </summary>
        [Description("The expected message address. This parameter is optional.")]
        public int? Address { get; set; }

        /// <summary>
        /// Returns an observable sequence of Harp messages matching the specified
        /// address and message type.
        /// </summary>
        /// <param name="source">An observable sequence of Harp messages.</param>
        /// <returns>
        /// An observable sequence of Harp messages matching the specified address
        /// and message type. If message type or address are <see langword="null"/>,
        /// messages of any type or from any address, respectively, are accepted.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            var address = Address;
            var messageType = MessageType;
            if (address == null && messageType == null) return source;
            if (address == null) return source.Where(messageType.Value);
            if (messageType == null) return source.Where(address.Value);
            return source.Where(address.Value, messageType.Value);
        }
    }
}
