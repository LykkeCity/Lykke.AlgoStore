using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class UserEntity: TableEntity
    {
        public bool GDPRConsent { get; set; }
        public bool CookieConsent { get; set; }
    }
}
