using System;

namespace Lykke.AlgoStore.Core.Utils
{
    public static class EnumExtensions
    {
        public static string ToUpperText(this Enum en)
        {
            return en.ToString("g").ToUpper();
        }
    }
}
