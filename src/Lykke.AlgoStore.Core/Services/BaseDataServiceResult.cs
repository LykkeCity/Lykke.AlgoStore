namespace Lykke.AlgoStore.Core.Services
{
    public class BaseDataServiceResult<T> : BaseServiceResult
    {
        public T Data { get; set; }
    }
}
