using System.Collections.Generic;

namespace Lykke.AlgoStore.Core.Utils
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmptyCollection<T>(this ICollection<T> collection)
        {
            if (collection == null)
                return true;

            if (collection.Count == 0)
                return true;

            return false;
        }
    }
}
