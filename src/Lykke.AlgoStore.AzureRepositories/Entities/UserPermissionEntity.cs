﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class UserPermissionEntity: TableEntity
    {
        public string DisplayName { get; set; }
    }
}
