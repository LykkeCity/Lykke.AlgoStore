using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class UserRoleEntity: TableEntity
    {
        public bool CanBeDeleted { get; set; }
        public bool CanBeModified { get; set; }
    }
}
