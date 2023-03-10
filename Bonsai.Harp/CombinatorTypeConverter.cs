using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides a type converter to convert combinator objects to and from other representations.
    /// </summary>
    class CombinatorTypeConverter : TypeConverter
    {
        internal static IEnumerable<Type> GetInstanceTypes(ITypeDescriptorContext context)
        {
            var builderType = context.Instance?.GetType() ?? context.PropertyDescriptor.ComponentType;
            var includeAttributes = (XmlIncludeAttribute[])builderType.GetCustomAttributes(typeof(XmlIncludeAttribute), inherit: true);
            if (includeAttributes.Length > 0)
            {
                return includeAttributes.Select(attribute => attribute.Type);
            }

            var propertyInfo = builderType.GetProperty(context.PropertyDescriptor.Name);
            var elementAttributes = (XmlElementAttribute[])propertyInfo.GetCustomAttributes(typeof(XmlElementAttribute), inherit: true);
            if (elementAttributes.Length > 0)
            {
                return elementAttributes.Select(attribute => attribute.Type);
            }

            return propertyInfo.PropertyType.GetCustomAttributes<XmlIncludeAttribute>().Select(attribute => attribute.Type);
        }

        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string typeName)
            {
                return GetInstanceTypes(context).FirstOrDefault(
                    type => string.Equals(type.Name, typeName, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Type valueType)
            {
                return valueType.Name;
            }

            return null;
        }

        /// <inheritdoc/>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <inheritdoc/>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <inheritdoc/>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var includeTypes = GetInstanceTypes(context).ToArray();
            return new StandardValuesCollection(includeTypes);
        }
    }
}
