using System.Collections.Generic;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class ClientAlgoMetaData
    {
        public string ClientId { get; set; }
        public List<AlgoMetaData> AlgosData { get; set; }
    }
}
