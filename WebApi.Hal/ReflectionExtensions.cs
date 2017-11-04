using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace WebApi.Hal
{
    internal static class ReflectionExtensions
    {
        static readonly string[] NonSerializedProperties = new[] { "Rel", "Href", "LinkName" };

        public static bool IsValidBasicType(this PropertyInfo property)
        {
            return !NonSerializedProperties.Contains(property.Name) && property.PropertyType.Namespace == "System"
                   && (property.PropertyType.GetTypeInfo().IsValueType || property.PropertyType == typeof(string));
        }

        public static bool IsGenericListOfApiResource(this Type type)
        {
            if (type.GetTypeInfo().IsGenericType && typeof(IList).IsAssignableFrom(type))
            {
                var genericType = type.GetGenericArguments().Single();
                return typeof(Representation).IsAssignableFrom(genericType);
            }

            return false;
        }

        public static PropertyInfo[] GetPublicInstanceProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);
        }

        public static void SetPropertyValueFromString(this Type type, string propertyName, string value, object instance)
        {
            var property = type.GetProperty(propertyName);

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            {
                property.SetPropertyValueFromString(Convert.ToInt32, value, instance);
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetPropertyValueFromString(Convert.ToString, value, instance);
            }
            else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
            {
                property.SetPropertyValueFromString(Convert.ToDateTime, value, instance);
            }
            else if (property.PropertyType == typeof(short) || property.PropertyType == typeof(short?))
            {
                property.SetPropertyValueFromString(Convert.ToInt16, value, instance);
            }
            else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
            {
                property.SetPropertyValueFromString(Convert.ToDecimal, value, instance);
            }
            else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            {
                property.SetPropertyValueFromString(Convert.ToBoolean, value, instance);
            }
            else if (property.PropertyType == typeof(byte) || property.PropertyType == typeof(byte?))
            {
                property.SetPropertyValueFromString(Convert.ToByte, value, instance);
            }
            else if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
            {
                property.SetPropertyValueFromString(Convert.ToInt64, value, instance);
            }
            else
            {
                throw new NotImplementedException("ResourceModel.ReflectionExtensions.SetPropertyValueFromString(...) does not yet support this data type: " + property.PropertyType);
            }
        }

        public static void SetPropertyValue(this Type type, string propertyName, XElement element, object instance)
        {
            if (element == null)
            {
                return;
            }

            type.SetPropertyValueFromString(propertyName, element.Value, instance);
        }

        public static void SetPropertyValue(this Type type, string propertyName, XAttribute element, object instance)
        {
            if (element == null)
            {
                return;
            }

            type.SetPropertyValueFromString(propertyName, element.Value, instance);
        }

        static void SetPropertyValueFromString<T>(this PropertyInfo property, Func<string, T> conversion, string value, object instance)
        {
            T convertedValue = value == null ? default(T) : conversion(value);

            property.SetValue(instance, convertedValue, null);
        }
    }
}