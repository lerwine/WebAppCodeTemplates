using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Erwine.Leonard.T.LoggingModule.ExtensionMethods
{
    public static class TypeExtensions
    {
        public static readonly Regex GenericNameEnding = new Regex(@"(~\d+)+$", RegexOptions.Compiled);

        public static Type[] GetAllElementTypes(this IEnumerable enumerable)
        {
            if (enumerable == null)
                return null;

            Type elementType = enumerable.GetType().GetEnumeratedElementType();
            if (elementType != null && (elementType.Equals(typeof(string)) || elementType.GetNullableUnderlyingOrType().IsValueType))
                return new Type[] { elementType };

            Type[] allDistinct = enumerable.Cast<object>().Where(o => o != null).Select(o => o.GetType()).Distinct().ToArray();
            if (allDistinct.Length == 0 && elementType != null)
                return new Type[] { elementType };

            if (allDistinct.All(t => t.IsValueType) && enumerable.Cast<object>().Any(o => o == null))
                return allDistinct.Concat(new Type[] { typeof(object) }).ToArray();

            return allDistinct;
        }

        public static Type GetNullableUnderlyingOrType(this Type type)
        {
            if (type != null && type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                return Nullable.GetUnderlyingType(type);

            return type;
        }

        public static Type GetDictionaryKeyType(this Type type)
        {
            if (type == null)
                return null;

            Type g = type.GetInterfacesOfGenericType(typeof(IDictionary<,>)).FirstOrDefault();
            if (g != null)
                return g.GetGenericArguments()[0];

            g = typeof(IDictionary);
            if (type.GetInterfaces().Any(t => t.Equals(g)))
                return typeof(object);

            return null;
        }

        public static Type GetDictionaryValueType(this Type type)
        {
            if (type == null)
                return null;

            Type g = type.GetInterfacesOfGenericType(typeof(IDictionary<,>)).FirstOrDefault();
            if (g != null)
                return g.GetGenericArguments()[1];

            g = typeof(IDictionary);
            if (type.GetInterfaces().Any(t => t.Equals(g)))
                return typeof(object);

            return null;
        }

        public static Type GetEnumeratedElementType(this Type type)
        {
            if (type == null)
                return null;

            if (type.IsArray)
                return type.GetElementType();

            Type g = type.GetInterfacesOfGenericType(typeof(IEnumerable<>)).FirstOrDefault();
            if (g != null)
                return g.GetGenericArguments()[0];

            g = typeof(IEnumerable);
            if (type.GetInterfaces().Any(t => t.Equals(g)))
                return typeof(object);

            return null;
        }

        public static IEnumerable<Type> GetInterfacesOfGenericType(this Type type, Type genericTypeDefinition)
        {
            Type g = (genericTypeDefinition.IsGenericTypeDefinition) ? genericTypeDefinition : genericTypeDefinition.GetGenericTypeDefinition();
            return type.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition().Equals(g));
        }

        public static string ToXmlElementName(this Type type)
        {
            string elementName = (type == null || type.Equals(typeof(object))) ? "Any" : TypeExtensions.GenericNameEnding.Replace(type.Name, "");

            return (type.IsArray && elementName != "Array") ? String.Format("ArrayOf{0}", elementName.Substring(elementName.Length - 2)) :
                elementName;
        }

        public static string ToCSharpTypeName(this Type type)
        {
            if (type == null || type.Equals(typeof(object)))
                return "object";

            string typeName;

            if (type.IsArray)
            {
                int n = type.GetArrayRank();
                typeName = type.GetElementType().ToCSharpTypeName();
                for (int i = 0; i < n; i++)
                    typeName += "[]";

                return typeName;
            }

            if (type.IsPrimitive)
            {
                TypeCode tc = Type.GetTypeCode(type);
                switch (tc)
                {
                    case TypeCode.Boolean:
                        return "bool";
                    case TypeCode.Int16:
                        return "short";
                    case TypeCode.Int32:
                        return "int";
                    case TypeCode.Int64:
                        return "long";
                    case TypeCode.Single:
                        return "float";
                    case TypeCode.UInt16:
                        return "ushort";
                    case TypeCode.UInt32:
                        return "uint";
                    case TypeCode.UInt64:
                        return "ulong";
                    case TypeCode.DateTime:
                    case TypeCode.DBNull:
                    case TypeCode.Empty:
                        return tc.ToString("F");
                    default:
                        return tc.ToString("F").ToLower();
                }
            }

            typeName = TypeExtensions.GenericNameEnding.Replace(type.FullName, "");

            if (type != null && type.IsGenericType)
                return String.Format("{0}<{1}>", typeName, String.Join(", ", type.GetGenericArguments().Select(a => a.ToCSharpTypeName()).ToArray()));

            return typeName;
        }

    }
}
