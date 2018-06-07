using Lykke.AlgoStore.Core.Enumerators;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoInstanceEntity : TableEntity
    {
        public string ClientId { get; set; }
        public string WalletId { get; set; }
        public AlgoInstanceStatus AlgoInstanceStatus { get; set; }

        //add more properties if needed

    }
}
