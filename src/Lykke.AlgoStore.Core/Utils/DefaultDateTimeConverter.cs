using Lykke.AlgoStore.Core.Constants;
using Newtonsoft.Json.Converters;

namespace Lykke.AlgoStore.Core.Utils
{
    public class DefaultDateTimeConverter : IsoDateTimeConverter
    {
        public DefaultDateTimeConverter()
        {
            base.DateTimeFormat = AlgoStoreConstants.DateTimeFormat;
        }
    }
}
