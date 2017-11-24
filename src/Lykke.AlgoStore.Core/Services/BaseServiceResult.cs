using Lykke.AlgoStore.Core.Domain.Errors;

namespace Lykke.AlgoStore.Core.Services
{
    public class BaseServiceResult
    {
        public BaseServiceResult()
        {
            ResultError = new AlgoStoreError { ErrorCode = 0, Description = string.Empty };
        }

        public AlgoStoreError ResultError { get; set; }
        public bool HasError
        {
            get
            {
                return (ResultError != null) && (ResultError.ErrorCode != AlgoStoreErrorCodes.None);
            }
        }
    }
}
