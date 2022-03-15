using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace A.Extensions
{
    public static class ATypeExtensions
    {
        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsCollectionType(this Type type)
        {
            return type != typeof(string) && type.GetInterface(nameof(IEnumerable)) != null;
        }

        public static bool IsAutoProperty(this PropertyInfo prop)
        {
            return prop.DeclaringType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                     .Any(f => f.Name.Contains("<" + prop.Name + ">"));
        }

        public static FieldInfo GetBackingField(this PropertyInfo prop)
        {
            return prop.DeclaringType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).First(f => f.Name.Contains("<" + prop.Name + ">"));
        }
    }
}