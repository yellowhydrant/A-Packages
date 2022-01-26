using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace A.Extensions
{
    public static class AEnumExtensions
    {
        private static void CheckIsEnum<TEnum>(bool withFlags)
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(TEnum).FullName));
            if (withFlags && !Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Type '{0}' doesn'TEnum have the 'Flags' attribute", typeof(TEnum).FullName));
        }

        public static bool IsFlagSet<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
        {
            CheckIsEnum<TEnum>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum value) where TEnum : Enum
        {
            CheckIsEnum<TEnum>(true);
            foreach (TEnum flag in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                if (value.IsFlagSet(flag))
                    yield return flag;
            }
        }

        public static TEnum SetFlags<TEnum>(this TEnum value, TEnum flags, bool on) where TEnum : Enum
        {
            CheckIsEnum<TEnum>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flags);
            if (on)
            {
                lValue |= lFlag;
            }
            else
            {
                lValue &= (~lFlag);
            }
            return (TEnum)Enum.ToObject(typeof(TEnum), lValue);
        }

        public static TEnum SetFlags<TEnum>(this TEnum value, TEnum flags) where TEnum : Enum
        {
            return value.SetFlags(flags, true);
        }

        public static TEnum ClearFlags<TEnum>(this TEnum value, TEnum flags) where TEnum : Enum
        {
            return value.SetFlags(flags, false);
        }

        public static TEnum CombineFlags<TEnum>(this IEnumerable<TEnum> flags) where TEnum : Enum
        {
            CheckIsEnum<TEnum>(true);
            long lValue = 0;
            foreach (TEnum flag in flags)
            {
                long lFlag = Convert.ToInt64(flag);
                lValue |= lFlag;
            }
            return (TEnum)Enum.ToObject(typeof(TEnum), lValue);
        }

        public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
        {
            CheckIsEnum<TEnum>(false);
            string name = Enum.GetName(typeof(TEnum), value);
            if (name != null)
            {
                FieldInfo field = typeof(TEnum).GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static int GetMaxValue<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<int>().Max();
        }

        public static int GetMinValue<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<int>().Min();
        }

        public static string AddWhiteSpace<TEnum>(this TEnum value, char charToReplace = '_') where TEnum : Enum
        {
            return value.ToString().Replace(charToReplace, ' ');
        }

        public static string SplitCamelCase<TEnum>(this TEnum value) where TEnum : Enum
        {
            return value.ToString().SplitCamelCase();
        }
    }
}