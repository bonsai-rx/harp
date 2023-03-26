using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Xml.Serialization;
using Bonsai.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Represents an operator that filters register-specific messages
    /// reported by the Harp device.
    /// </summary>
    [XmlInclude(typeof(TypeMapping<FilterMessageAddress>))]
    [XmlInclude(typeof(TypeMapping<WhoAmI>))]
    [XmlInclude(typeof(TypeMapping<HardwareVersionHigh>))]
    [XmlInclude(typeof(TypeMapping<HardwareVersionLow>))]
    [XmlInclude(typeof(TypeMapping<AssemblyVersion>))]
    [XmlInclude(typeof(TypeMapping<CoreVersionHigh>))]
    [XmlInclude(typeof(TypeMapping<CoreVersionLow>))]
    [XmlInclude(typeof(TypeMapping<FirmwareVersionHigh>))]
    [XmlInclude(typeof(TypeMapping<FirmwareVersionLow>))]
    [XmlInclude(typeof(TypeMapping<TimestampSeconds>))]
    [XmlInclude(typeof(TypeMapping<TimestampMicroseconds>))]
    [XmlInclude(typeof(TypeMapping<OperationControl>))]
    [XmlInclude(typeof(TypeMapping<ResetDevice>))]
    [XmlInclude(typeof(TypeMapping<DeviceName>))]
    [XmlInclude(typeof(TypeMapping<SerialNumber>))]
    [XmlInclude(typeof(TypeMapping<ClockConfiguration>))]
    [Description("Filters register-specific messages reported by the Device device.")]
    public class FilterMessage : FilterMessageBuilder, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMessage"/> class.
        /// </summary>
        public FilterMessage()
        {
            Register = new TypeMapping<FilterMessageAddress>();
        }

        string INamedElement.Name => Register is TypeMapping<FilterMessageAddress>
            ? default
            : $"Device.{GetElementDisplayName(Register?.GetType().GenericTypeArguments[0])}";

        /// <summary>
        /// Gets or sets the desired message address. This parameter is optional.
        /// </summary>
        [Description("The desired message address. This parameter is optional.")]
        public int? Address { get; set; }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var register = Register;
            var messageType = MessageType;
            var source = arguments.First();
            if (register == null)
            {
                throw new InvalidOperationException("The target register type cannot be null.");
            }

            Expression[] filterArguments;
            var registerType = register.GetType().GenericTypeArguments[0];
            if (registerType == typeof(FilterMessageAddress))
            {
                var address = Address;
                if (address == null && messageType == null) return source;
                if (address == null) filterArguments = new[] { source, Expression.Constant(messageType.Value) };
                if (messageType == null) filterArguments = new[] { source, Expression.Constant(address.Value) };
                else filterArguments = new[] { source, Expression.Constant(address.Value), Expression.Constant(messageType.Value) };
            }
            else
            {
                var registerAddress = Expression.Field(null, registerType, nameof(HarpMessage.Address));
                filterArguments = messageType.HasValue
                    ? new[] { source, registerAddress, Expression.Constant(messageType.Value) }
                    : new[] { source, registerAddress };
            }

            return Expression.Call(
                typeof(ObservableExtensions),
                nameof(ObservableExtensions.Where),
                typeArguments: null,
                filterArguments);
        }
    }

    /// <summary>
    /// Represents an operator which filters a sequence of Harp messages for elements that match the specified address and message type.
    /// </summary>
    [DesignTimeVisible(false)]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    [Description("Filters a sequence of Harp messages for elements that match the specified address and message type.")]
    public class FilterMessageAddress : Combinator<HarpMessage, HarpMessage>
    {
        /// <summary>
        /// Gets or sets the desired message address. This parameter is optional.
        /// </summary>
        [Description("The desired message address. This parameter is optional.")]
        public int? Address { get; set; }

        /// <summary>
        /// Gets or sets the desired type of the message. This parameter is optional.
        /// </summary>
        [Description("The desired type of the message. This parameter is optional.")]
        public MessageType? MessageType { get; set; }

        /// <summary>
        /// Returns an observable sequence of Harp messages matching the specified address and message type.
        /// </summary>
        /// <param name="source">An observable sequence of Harp messages.</param>
        /// <returns>
        /// An observable sequence of Harp messages matching the specified address and message type. If
        /// <c>Address</c> or <c>MessageType</c> are <c>null</c>, any address or message type, respectively,
        /// are accepted.
        /// </returns>
        public override IObservable<HarpMessage> Process(IObservable<HarpMessage> source)
        {
            var address = Address;
            var messageType = MessageType;
            if (address == null && messageType == null) return source;
            if (address == null) return source.Where(input => input.MessageType == messageType);
            if (messageType == null) return source.Where(input => input.Address == address);
            return source.Where(input => input.Address == address && input.MessageType == messageType);
        }
    }
}
