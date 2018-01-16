using System.Collections.Generic;

namespace Lykke.AlgoStore.Services.Utils
{
    public static class SourceCodeTypeHelper
    {
        private const string KeyFormat = "{0}.{1}";
        private static readonly Dictionary<SourceCodeTypes, string> SupportedExtensions =
            new Dictionary<SourceCodeTypes, string>
            {
                {SourceCodeTypes.Unknown, "" },
                {SourceCodeTypes.JavaBinary, "jar" },
                {SourceCodeTypes.JavaSource, "java" }
            };

        public static string GetBlobKey(string key, SourceCodeTypes type)
        {
            if (!SupportedExtensions.TryGetValue(type, out string ext))
                ext = SupportedExtensions[SourceCodeTypes.Unknown];

            return string.Format(KeyFormat, key, ext);
        }
    }
}
