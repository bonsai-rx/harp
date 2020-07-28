using System;
using System.ComponentModel;
using System.Reflection;

namespace Bonsai.Harp
{
    public class DeviceTypeDescriptionProvider<TElement> : TypeDescriptionProvider
    {
        static readonly TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(typeof(TElement));

        public DeviceTypeDescriptionProvider()
            : base(parentProvider)
        {
        }

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
                descriptionProperty = instance.GetType().GetProperty("Description", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); ;
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
