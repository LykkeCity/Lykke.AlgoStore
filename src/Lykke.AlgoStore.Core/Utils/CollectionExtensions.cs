using System.Collections;

namespace Lykke.AlgoStore.Core.Utils
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmptyCollection(this ICollection collection)
        {
            if (collection == null)
                return true;

            if (collection.Count == 0)
                return true;

            return false;
        }

        public static bool IsNullOrEmptyEnumerable(this IEnumerable enumerable)
        {
            if (enumerable == null)
                return true;

            var enumerator = enumerable.GetEnumerator();
            enumerator.Reset();
            if (enumerator.MoveNext())
            {
                enumerator.Reset();
                return false;
            }

            return true;
        }
    }
}
