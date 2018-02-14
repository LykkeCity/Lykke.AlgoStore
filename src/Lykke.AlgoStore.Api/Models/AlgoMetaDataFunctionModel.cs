using System.Collections.Generic;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoMetaDataFunctionModel
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public IEnumerable<AlgoMetaDataParameterModel> Parameters { get; set; }
    }
}
