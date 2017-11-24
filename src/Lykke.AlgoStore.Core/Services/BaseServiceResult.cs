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

        public static BaseServiceResult CreateFromError(AlgoStoreErrorCodes errorCode)
        {
            return CreateFromError(errorCode, string.Empty);
        }

        public static BaseServiceResult CreateFromError(AlgoStoreErrorCodes errorCode, string message)
        {
            var result = new BaseServiceResult();
            result.ResultError.ErrorCode = errorCode;
            result.ResultError.Description = message;
            return result;
        }
    }
}
