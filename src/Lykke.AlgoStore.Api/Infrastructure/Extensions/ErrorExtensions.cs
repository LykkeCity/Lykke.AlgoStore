using System.Net;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Constants;
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
            errorResponse.DisplayMessage = string.IsNullOrEmpty(error.DisplayMessage)
                ? AlgoStoreConstants.DefaultDisplayMessage
                : error.DisplayMessage;

            HttpStatusCode statusCode;

            switch (error.ErrorCode)
            {
                case AlgoStoreErrorCodes.ValidationError:
                case AlgoStoreErrorCodes.RuntimeSettingsExists:
                case AlgoStoreErrorCodes.AlgoInsatncesCountLimit:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case AlgoStoreErrorCodes.AlgoNotFound:
                case AlgoStoreErrorCodes.NotFound:
                case AlgoStoreErrorCodes.AlgoBinaryDataNotFound:
                case AlgoStoreErrorCodes.AlgoRuntimeDataNotFound:
                case AlgoStoreErrorCodes.PodNotFound:
                case AlgoStoreErrorCodes.AssetNotFound:
                case AlgoStoreErrorCodes.AlgoInstanceDataNotFound:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case AlgoStoreErrorCodes.Conflict:
                    statusCode = HttpStatusCode.Conflict;
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
