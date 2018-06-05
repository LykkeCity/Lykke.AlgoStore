using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Lykke.AlgoStore.Core.Utils
{
    public static class EnumExtensions
    {
        public static string ToUpperText(this Enum en)
        {
            return en.ToString("g").ToUpper();
        }

        public static string GetDisplayName(this Enum val)
        {
            return val.GetType()
                       .GetMember(val.ToString())
                       .FirstOrDefault()
                       ?.GetCustomAttribute<DisplayAttribute>(false)
                       ?.Name
                   ?? val.ToString();
        }
    }
}
