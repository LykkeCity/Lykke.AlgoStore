using System.Collections.Generic;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoMetaDataInformationModel
    {
        public IEnumerable<AlgoMetaDataParameterModel> Parameters { get; set; }
        public IEnumerable<AlgoMetaDataFunctionModel> Functions { get; set; }
    }
}
