using System;
using Lykke.AlgoStore.Core.Enumerators;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoMetaDataEntity : TableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string AlgoVisibilityValue { get; set; }
        public string AlgoMetaDataInformationJSON { get; set; }

        public AlgoVisibility AlgoVisibility
        {
            get
            {
                Enum.TryParse(AlgoVisibilityValue, out AlgoVisibility visibility);
                return visibility;
            }
            set => AlgoVisibilityValue = value.ToString();
        }
    }
}
