using System;
using System.Linq;
using System.Reflection;

namespace Erwine.Leonard.T.ExtensionMethods.AttributeTypes
{
    public static class AttributeExtensions
    {
        public static TAttribute[] GetAttributesOfType<TAttribute>(this MemberInfo memberInfo, bool inherit)
            where TAttribute : Attribute
        {
            if (memberInfo == null)
                return new TAttribute[0];

            return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>().ToArray();
        }
    }
}
