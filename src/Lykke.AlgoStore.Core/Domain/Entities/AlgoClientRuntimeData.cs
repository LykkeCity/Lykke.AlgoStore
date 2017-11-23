using System.Collections.Generic;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoClientRuntimeData
    {
        public string ClientAlgoId { get; set; }
        public List<AlgoRuntimeData> RuntimeData { get; set; }
    }
}
