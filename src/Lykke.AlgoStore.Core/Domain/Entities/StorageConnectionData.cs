namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class StorageConnectionData
    {
        public string DateHeader { get; set; }
        public string VersionHeader { get; set; }
        public string AuthorizationHeader { get; set; }
        public string StorageAccountName { get; set;}
        public string ContainerName { get; set; }
        public string AccessKey { get; set; }
        public string Url { get; set; }
    }
}
