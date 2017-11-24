using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.AzureRepositories.Entities
{
    public class AlgoTemplateDataEntity : TableEntity
    {
        public string LanguageId { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Version { get; set; }
        public string Branch { get; set; }
        public string Build { get; set; }
        public bool IsActive { get; set; }
    }
}
