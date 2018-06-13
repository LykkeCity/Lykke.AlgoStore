namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoClientRuntimeData
    {
        public string AlgoId { get; set; }
        public string ClientId { get; set; }
        public int BuildId { get; set; }
        public string PodId { get; set; }
    }
}
