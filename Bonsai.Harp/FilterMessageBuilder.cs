using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using Bonsai.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to filter
    /// a sequence of Harp messages for elements that match the specified register
    /// and message type.
    /// </summary>
    [DefaultProperty(nameof(Register))]
    [XmlType(Namespace = Constants.XmlNamespace)]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    public abstract class FilterMessageBuilder : SingleArgumentExpressionBuilder, ICustomTypeDescriptor
    {
        /// <summary>
        /// Gets or sets the register used to filter Harp device messages.
        /// </summary>
        [DesignOnly(true)]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [TypeConverter(typeof(CombinatorTypeMappingConverter))]
        [Description("The register used to filter Harp device messages.")]
        public TypeMapping Register { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the type of Harp device message to filter.
        /// This parameter is optional.
        /// </summary>
        [Category(nameof(CategoryAttribute.Design))]
        [Description("Specifies the type of Harp device message to filter. This parameter is optional.")]
        public MessageType? MessageType { get; set; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="MessageType"/> property
        /// should be serialized.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the <see cref="MessageType"/> should be serialized;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool ShouldSerializeMessageType() => MessageType.HasValue;

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

            var registerType = register.GetType().GenericTypeArguments[0];
            var registerAddress = Expression.Field(null, registerType, nameof(HarpMessage.Address));
            var filterArguments = messageType.HasValue
                ? new[] { source, registerAddress, Expression.Constant(messageType.Value) }
                : new[] { source, registerAddress };
            return Expression.Call(
                typeof(ObservableExtensions),
                nameof(ObservableExtensions.Where),
                typeArguments: null,
                filterArguments);
        }

        class CombinatorTypeMappingConverter : CombinatorTypeConverter
        {
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string typeName)
                {
                    var includeType = GetInstanceTypes(context).FirstOrDefault(
                        type => string.Equals(type.GenericTypeArguments[0].Name, typeName, StringComparison.OrdinalIgnoreCase));
                    if (includeType != null) return Activator.CreateInstance(includeType);
                }

                return null;
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string) && value is TypeMapping valueType)
                {
                    return valueType.GetType().GenericTypeArguments[0].Name;
                }

                return null;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var includeTypes = GetInstanceTypes(context).Select(Activator.CreateInstance).ToArray();
                return new StandardValuesCollection(includeTypes);
            }
        }

        #region ICustomTypeDescriptor Members

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            var attributes = TypeDescriptor.GetAttributes(GetType());
            var defaultProperty = TypeDescriptor.GetDefaultProperty(GetType());
            if (defaultProperty != null)
            {
                var instance = defaultProperty.GetValue(this);
                if (instance is TypeMapping)
                {
                    var mappingAttributes = TypeDescriptor.GetAttributes(instance.GetType().GenericTypeArguments[0]);
                    if (mappingAttributes[typeof(DescriptionAttribute)] is DescriptionAttribute description)
                    {
                        return AttributeCollection.FromExisting(attributes, description);
                    }
                }
            }

            return attributes;
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(GetType());
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(GetType());
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(GetType());
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(GetType(), editorBaseType);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            var properties = TypeDescriptor.GetProperties(GetType(), attributes);
            var defaultProperty = TypeDescriptor.GetDefaultProperty(GetType());
            if (defaultProperty != null)
            {
                var instance = defaultProperty.GetValue(this);
                var includeAddress = instance is TypeMapping<FilterMessageAddress>;
                return new PropertyDescriptorCollection(properties
                    .Cast<PropertyDescriptor>()
                    .Where(property => includeAddress || property.Name != nameof(FilterMessageAddress.Address))
                    .ToArray());
            }

            return properties;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}
