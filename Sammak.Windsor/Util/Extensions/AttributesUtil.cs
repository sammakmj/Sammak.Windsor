using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Sammak.Windsor.Util.Extensions
{
    /// <summary>Helper class for retrieving attributes.</summary>
    public static class AttributesUtil
    {
        /// <summary>Gets the attribute.</summary>
        /// <param name="member">The member.</param>
        /// <returns>The member attribute.</returns>
        public static T GetAttribute<T>(this ICustomAttributeProvider member) where T : class
        {
            return ((IEnumerable<T>)member.GetAttributes<T>()).FirstOrDefault<T>();
        }

        /// <summary>
        ///   Gets the attributes. Does not consider inherited attributes!
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>The member attributes.</returns>
        public static T[] GetAttributes<T>(this ICustomAttributeProvider member) where T : class
        {
            if (typeof(T) != typeof(object))
                return (T[])member.GetCustomAttributes(typeof(T), false);
            return (T[])member.GetCustomAttributes(false);
        }

        /// <summary>Gets the type attribute.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The type attribute.</returns>
        public static T GetTypeAttribute<T>(this Type type) where T : class
        {
            T obj = type.GetAttribute<T>();
            if ((object)obj == null)
            {
                foreach (Type type1 in type.GetInterfaces())
                {
                    obj = type1.GetTypeAttribute<T>();
                    if ((object)obj != null)
                        break;
                }
            }
            return obj;
        }

        /// <summary>Gets the type attributes.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The type attributes.</returns>
        public static T[] GetTypeAttributes<T>(Type type) where T : class
        {
            T[] objArray = type.GetAttributes<T>();
            if (objArray.Length == 0)
            {
                foreach (Type type1 in type.GetInterfaces())
                {
                    objArray = AttributesUtil.GetTypeAttributes<T>(type1);
                    if (objArray.Length > 0)
                        break;
                }
            }
            return objArray;
        }

        /// <summary>Gets the type converter.</summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static Type GetTypeConverter(MemberInfo member)
        {
            TypeConverterAttribute attribute = member.GetAttribute<TypeConverterAttribute>();
            if (attribute != null)
                return Type.GetType(attribute.ConverterTypeName);
            return (Type)null;
        }

        /// <summary>Gets the attribute.</summary>
        /// <param name="member">The member.</param>
        /// <returns>The member attribute.</returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider member) where T : class
        {
            return (object)((IEnumerable<T>)member.GetAttributes<T>()).FirstOrDefault<T>() != null;
        }
    }
}