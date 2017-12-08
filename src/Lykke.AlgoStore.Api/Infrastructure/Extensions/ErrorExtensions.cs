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
            var errorResponse = new BaseErrorResponse();

            var aggregate = error as AlgoStoreAggregateException;
            if (aggregate != null)
            {
                errorResponse = new ErrorResponse();
                ((ErrorResponse)errorResponse).ModelErrors = aggregate.Errors;
            }

            errorResponse.ErrorCode = (int)error.ErrorCode;
            errorResponse.ErrorDescription = error.ErrorCode.ToString("g");
            errorResponse.ErrorMessage = error.Message;

            HttpStatusCode statusCode;

            switch (error.ErrorCode)
            {
                case AlgoStoreErrorCodes.ValidationError:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case AlgoStoreErrorCodes.AlgoNotFound:
                case AlgoStoreErrorCodes.AlgoBinaryDataNotFound:
                case AlgoStoreErrorCodes.AlgoRuntimeDataNotFound:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;

            }

            var result = new ObjectResult(errorResponse);
            result.StatusCode = (int)statusCode;

            return result;
        }
    }
}
