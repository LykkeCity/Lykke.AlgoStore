using System;

namespace Lykke.AlgoStore.Core.Domain.Errors
{
    public class AlgoStoreException : Exception
    {
        const string ErrorMessageFormat = "Code:{0}-{1} Message:{2}";

        public AlgoStoreException(AlgoStoreErrorCodes errorCode) : this(errorCode, string.Empty)
        { }
        public AlgoStoreException(AlgoStoreErrorCodes errorCode, Exception exception) : this(errorCode, string.Empty, exception)
        { }
        public AlgoStoreException(AlgoStoreErrorCodes errorCode, string errorMessage) : this(errorCode, errorMessage, null)
        { }

        public AlgoStoreException(AlgoStoreErrorCodes errorCode, string errorMessage, Exception exception) :
            base(string.Format(ErrorMessageFormat, errorCode.ToString("d"), errorCode.ToString("g"), errorMessage ?? string.Empty), exception)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public AlgoStoreErrorCodes ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
    }
}
