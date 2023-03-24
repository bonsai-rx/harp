﻿using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides the abstract base class for polymorphic operators used to manipulate
    /// Harp device messages.
    /// </summary>
    public abstract class HarpCombinatorBuilder : ExpressionBuilder, ICustomTypeDescriptor
    {
        readonly CombinatorBuilder builder = new CombinatorBuilder();

        internal HarpCombinatorBuilder()
        {
        }

        internal static string RemoveSuffix(string source, string suffix)
        {
            var suffixStart = source.LastIndexOf(suffix);
            return suffixStart >= 0 ? source.Remove(suffixStart) : source;
        }

        /// <inheritdoc/>
        public override Range<int> ArgumentRange => builder.ArgumentRange;

        /// <summary>
        /// Gets or sets the combinator instance used to process device messages.
        /// </summary>
        protected object Combinator
        {
            get { return builder.Combinator; }
            set { builder.Combinator = value; }
        }

        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            return builder.Build(arguments);
        }

        #region ICustomTypeDescriptor Members

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            var attributes = TypeDescriptor.GetAttributes(GetType());
            var defaultProperty = TypeDescriptor.GetDefaultProperty(GetType());
            if (defaultProperty != null)
            {
                var instance = defaultProperty.GetValue(this);
                var instanceAttributes = TypeDescriptor.GetAttributes(instance);
                if (instanceAttributes[typeof(DescriptionAttribute)] is DescriptionAttribute description)
                {
                    return AttributeCollection.FromExisting(attributes, description);
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
            var defaultProperty = TypeDescriptor.GetDefaultProperty(GetType());
            return defaultProperty != null ? new FactoryTypePropertyDescriptor(defaultProperty) : null;
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
            var defaultProperty = TypeDescriptor.GetDefaultProperty(GetType());
            if (defaultProperty != null)
            {
                var instance = defaultProperty.GetValue(this);
                var instanceProperties = TypeDescriptor.GetProperties(instance, attributes);
                var properties = new PropertyDescriptor[instanceProperties.Count + 1];
                properties[0] = new FactoryTypePropertyDescriptor(defaultProperty);
                for (int i = 0; i < instanceProperties.Count; i++)
                {
                    var expandedProperty = instanceProperties[i];
                    properties[i + 1] = expandedProperty;
                }
                return new PropertyDescriptorCollection(properties);
            }

            return TypeDescriptor.GetProperties(this, attributes);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return pd?.ComponentType.IsAssignableFrom(GetType()) == true ? this : builder.Combinator;
        }

        class FactoryTypePropertyDescriptor : PropertyDescriptor
        {
            readonly PropertyDescriptor descriptor;

            public FactoryTypePropertyDescriptor(PropertyDescriptor descr)
                : base(descr)
            {
                descriptor = descr;
            }

            public override Type ComponentType => descriptor.ComponentType;

            public override bool IsReadOnly => false;

            public override Type PropertyType => typeof(Type);

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override object GetValue(object component)
            {
                component = descriptor.GetValue(component);
                return component?.GetType();
            }

            public override void ResetValue(object component)
            {
                descriptor.SetValue(component, null);
            }

            public override void SetValue(object component, object value)
            {
                var currentValue = descriptor.GetValue(component);
                var newValue = Activator.CreateInstance((Type)value);

                var newProperties = TypeDescriptor.GetProperties(newValue);
                var currentProperties = TypeDescriptor.GetProperties(currentValue);
                foreach (PropertyDescriptor property in newProperties)
                {
                    var mergeProperty = currentProperties[property.Name];
                    if (mergeProperty?.PropertyType == property.PropertyType)
                    {
                        var propertyValue = mergeProperty.GetValue(currentValue);
                        property.SetValue(newValue, propertyValue);
                    }
                }

                descriptor.SetValue(component, newValue);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
        }

        #endregion
    }
}
