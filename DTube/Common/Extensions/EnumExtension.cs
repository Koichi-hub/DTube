using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Reflection;

namespace DTube.Common.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString()).FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?.GetName() ?? string.Empty;
        }
    }
}
