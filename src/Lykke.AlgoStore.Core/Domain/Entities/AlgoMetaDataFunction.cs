using System.Collections.Generic;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoMetaDataFunction
    {
        public string Type { get; set; }
        public string Id { get; set; }

        public IEnumerable<AlgoMetaDataParameter> Parameters { get; set; }
    }
}
