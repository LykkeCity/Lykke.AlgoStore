using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class UserPermissionEntity: TableEntity
    {
        public string DisplayName { get; set; }
    }
}
