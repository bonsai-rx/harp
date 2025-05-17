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
    /// the specified register address.
    /// </summary>
    /// <seealso cref="FilterRegisterAddress"/>
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
    [XmlInclude(typeof(FilterRegisterAddress))]
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
    [Description("Filters the sequence for Harp messages matching the specified register address.")]
    public class FilterRegister : FilterRegisterBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRegister"/> class.
        /// </summary>
        public FilterRegister()
        {
            Register = new FilterRegisterAddress();
        }

        string INamedElement.Name => Register is FilterRegisterAddress
            ? default
            : $"Device.{GetElementDisplayName(Register)}";

        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies how the message filter will use the matching criteria.")]
        public new FilterType FilterType
        {
            get { return base.FilterType; }
            set
            {
                base.FilterType = value;
                if (Register is FilterRegisterAddress filterMessage)
                {
                    filterMessage.FilterType = value;
                }
            }
        }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            if (Register is FilterRegisterAddress filterMessage)
            {
                var source = arguments.First();
                var combinator = Expression.Constant(filterMessage);
                filterMessage.FilterType = FilterType;
                return Expression.Call(combinator, nameof(FilterRegisterAddress.Process), null, source);
            }
            else return base.Build(arguments);
        }
    }

    /// <summary>
    /// Represents an operator that filters the sequence for Harp messages matching
    /// the specified address.
    /// </summary>
    [DesignTimeVisible(false)]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    [Description("Filters the sequence for Harp messages matching the specified address.")]
    public class FilterRegisterAddress : Combinator<HarpMessage, HarpMessage>
    {
        /// <summary>
        /// Gets or sets a value specifying how the message filter will use the matching criteria.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        [Description("Specifies how the message filter will use the matching criteria.")]
        public FilterType FilterType { get; set; }

        /// <summary>
        /// Gets or sets the expected message address. This parameter is optional.
        /// </summary>
        [Description("The expected message address. This parameter is optional.")]
        public int? Address { get; set; }

        /// <summary>
        /// Returns an observable sequence of Harp messages matching the specified
        /// address.
        /// </summary>
        /// <param name="source">An observable sequence of Harp messages.</param>
        /// <returns>
        /// An observable sequence of Harp messages matching the specified address.
        /// If no address is specified, all messages will be accepted.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            var address = Address;
            if (address == null) return source;
            else return FilterType == FilterType.Include
                ? source.Where(message => message.Address == address)
                : source.Where(message => message.Address != address);
        }
    }
}
