using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Erwine.Leonard.T.ExtensionMethods.AttributeTypes;

namespace Erwine.Leonard.T.LoggingModule.ExtensionMethods
{
    public static class DescriptionExtensions
    {
        public static string GetEnumDescription<TEnum>(this TEnum value)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            DescriptionAttribute da = value.GetEnumAttributes<TEnum, DescriptionAttribute>().FirstOrDefault(a => !String.IsNullOrWhiteSpace(a.Description));

            if (da != null)
                return da.Description;

            return Enum.GetName(value.GetType(), value).Replace("_", " ");
        }

    }
}
