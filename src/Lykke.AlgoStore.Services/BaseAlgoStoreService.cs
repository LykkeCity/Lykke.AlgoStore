using System;
using Common.Log;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Errors;

namespace Lykke.AlgoStore.Services
{
    public class BaseAlgoStoreService
    {
        public BaseAlgoStoreService(ILog log)
        {
            Log = log;
        }

        public ILog Log { get; }

        protected AlgoStoreException HandleException(Exception ex, string componentName)
        {
            var exception = ex as AlgoStoreException;

            if (exception == null)
                exception = new AlgoStoreException(AlgoStoreErrorCodes.Unhandled, ex);

            var validationException = exception as AlgoStoreAggregateException;
            if (validationException != null)
                Log.WriteErrorAsync(AlgoStoreConstants.ProcessName, componentName, validationException.ToBaseException()).Wait();
            else
                Log.WriteErrorAsync(AlgoStoreConstants.ProcessName, componentName, exception).Wait();

            return exception;
        }
    }
}
