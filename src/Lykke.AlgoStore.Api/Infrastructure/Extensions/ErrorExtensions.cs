using System.Net;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AlgoStore.Api.Infrastructure.Extensions
{
    public static class ErrorExtensions
    {
        public static ObjectResult ToHttpStatusCode(this AlgoStoreException error)
        {
            var errorModel = new ErrorModel
            {
                ErrorCode = (int)error.ErrorCode,
                ErrorDescription = error.ErrorCode.ToString("g"),
                ErrorMessage = error.ErrorMessage
            };

            var aggregate = error as AlgoStoreAggregateException;
            if (aggregate != null)
                errorModel.ModelErrors = aggregate.Errors;

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            switch (error.ErrorCode)
            {
                case AlgoStoreErrorCodes.ValidationError:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;

            }

            var result = new ObjectResult(errorModel);
            result.StatusCode = (int)statusCode;

            return result;
        }
    }
}
