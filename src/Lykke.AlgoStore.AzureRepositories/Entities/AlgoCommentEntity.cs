using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoCommentEntity: TableEntity
    {
        public string AutorId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
        public string Content { get; set; }
    }
}
