using System.Collections.Generic;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoMetaDataInformation
    {
        public IEnumerable<AlgoMetaDataParameter> Parameters { get; set; }
        public IEnumerable<AlgoMetaDataFunction> Functions { get; set; }
    }
}
