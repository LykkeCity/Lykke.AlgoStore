using System;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Core.Domain.Errors;

namespace Lykke.AlgoStore.Api.Infrastructure.Managers
{
    internal static class ExceptionManager
    {
        public static object CreateErrorResponse(Exception ex)
        {
            Exception temp = ex;

            var aggr = ex as AggregateException;
            if (aggr != null)
                temp = aggr.InnerExceptions[0];

            var exception = temp as AlgoStoreException;

            if (exception == null)
                exception = new AlgoStoreException(AlgoStoreErrorCodes.Unhandled, ex);

            return exception.ToHttpStatusCode();
        }
    }
}
