using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Core.Utils
{
    public static class StringExtensions
    {
        public const char CommaSeparator = ',';
        public const char ColonSeparator = ':';
        public const char SemiColonSeparator = ';';

        public static List<string> SplitTrimmed(this string stringList, char separator, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(stringList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return stringList
                .TrimEnd(separator)
                .Split(separator)
                .AsEnumerable()
                .Select(s => s.Trim())
                .ToList();
        }
    }
}
