using System;
using System.ComponentModel;
using System.Reflection;

namespace Bonsai.Harp
{
    /// <summary>
    /// Provides dynamic device description data based on a private or internal <c>Description</c> property.
    /// As long as such a property exists, this type description provider will use it to override the default
    /// <see cref="DescriptionAttribute"/> of the class.
    /// </summary>
    /// <typeparam name="TElement">The type of the element on which the attribute is applied.</typeparam>
    public sealed class DeviceTypeDescriptionProvider<TElement> : TypeDescriptionProvider
    {
        static readonly TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(typeof(TElement));

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceTypeDescriptionProvider{TElement}"/> class.
        /// </summary>
        public DeviceTypeDescriptionProvider()
            : base(parentProvider)
        {
        }

        /// <summary>
        /// Gets a custom type descriptor for the given type and object.
        /// </summary>
        /// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
        /// <param name="instance">An instance of the type. Can be null if no instance was passed to the type descriptor.</param>
        /// <returns>An <see cref="ICustomTypeDescriptor"/> that can provide metadata for the type.</returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            var parentDescriptor = base.GetTypeDescriptor(objectType, instance);
            if (instance != null) return new DeviceTypeInstanceDescriptor(instance, parentDescriptor);
            return parentDescriptor;
        }

        class DeviceTypeInstanceDescriptor : CustomTypeDescriptor
        {
            readonly object component;
            readonly PropertyInfo descriptionProperty;

            public DeviceTypeInstanceDescriptor(object instance, ICustomTypeDescriptor parentDescriptor)
                : base(parentDescriptor)
            {
                component = instance;
                descriptionProperty = instance.GetType().GetProperty("Description", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            public override AttributeCollection GetAttributes()
            {
                var attributes = base.GetAttributes();
                if (descriptionProperty != null && descriptionProperty.PropertyType == typeof(string))
                {
                    var description = (string)descriptionProperty.GetValue(component);
                    if (!string.IsNullOrEmpty(description))
                    {
                        return AttributeCollection.FromExisting(attributes, new DescriptionAttribute(description));
                    }
                }

                return attributes;
            }
        }
    }
}
