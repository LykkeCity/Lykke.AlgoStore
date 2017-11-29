using System;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Core.Domain.Errors;

namespace Lykke.AlgoStore.Infrastructure
{
    internal class ExceptionManager
    {
        private static readonly ExceptionManager _instance = new ExceptionManager();
        public static ExceptionManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public object CreateErrorResponse(Exception ex)
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
