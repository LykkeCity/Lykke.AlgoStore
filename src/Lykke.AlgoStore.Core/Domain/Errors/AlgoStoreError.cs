namespace Lykke.AlgoStore.Core.Domain.Errors
{
    public class AlgoStoreError
    {
        public AlgoStoreErrorCodes ErrorCode { get; set; }
        public string Description { get; set; }
    }
}
