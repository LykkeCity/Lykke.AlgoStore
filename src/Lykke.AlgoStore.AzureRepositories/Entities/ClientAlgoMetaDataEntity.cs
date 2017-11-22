using Lykke.AlgoStore.Core.Domain.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class ClientAlgoMetaDataEntity : TableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AlgoDataId { get; set; }
    }
}
