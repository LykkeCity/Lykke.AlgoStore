using System.Collections.Generic;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoData
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public IEnumerable<AlgoRuntimeSettings> RuntimeSettings { get; set; }
    }
}
