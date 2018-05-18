using System;

namespace Lykke.AlgoStore.Core.Domain.Errors
{
    public class AlgoStoreException : Exception
    {
        const string ErrorMessageFormat = "Code:{0}-{1} Message:{2}";

        public AlgoStoreException(AlgoStoreErrorCodes errorCode) : this(errorCode, string.Empty)
        { }
        public AlgoStoreException(AlgoStoreErrorCodes errorCode, Exception exception) : this(errorCode, string.Empty, string.Empty, exception)
        { }
        public AlgoStoreException(AlgoStoreErrorCodes errorCode, string errorMessage) : this(errorCode, errorMessage, string.Empty, null)
        { }
        public AlgoStoreException(AlgoStoreErrorCodes errorCode, string errorMessage, string displayMessage) : this(errorCode, errorMessage, displayMessage, null)
        { }

        public AlgoStoreException(AlgoStoreErrorCodes errorCode, string errorMessage, string displayMessage, Exception exception) :
            base(string.Format(ErrorMessageFormat, errorCode.ToString("d"), errorCode.ToString("g"), errorMessage ?? string.Empty), exception)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage ?? string.Empty;
            DisplayMessage = displayMessage;
        }

        public AlgoStoreErrorCodes ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public string DisplayMessage { get; private set; }
    }
}
