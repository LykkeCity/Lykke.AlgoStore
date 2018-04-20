using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class UserRoleEntity: TableEntity
    {
        public bool CanBeDeleted { get; set; }
        public bool CanBeModified { get; set; }
    }
}
