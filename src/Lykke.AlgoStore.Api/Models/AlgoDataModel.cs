using Lykke.AlgoStore.Core.Enumerators;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoDataModel
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }      
        public AlgoVisibility AlgoVisibility { get; set; }
    }
}
