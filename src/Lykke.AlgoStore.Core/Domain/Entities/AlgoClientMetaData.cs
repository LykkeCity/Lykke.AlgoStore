using System.Collections.Generic;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoClientMetaData
    {
        public string ClientId { get; set; }
        public string Author { get; set; }
        public List<AlgoMetaData> AlgoMetaData { get; set; }
    }
}
