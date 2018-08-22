using System.Net;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Errors;
using Microsoft.AspNetCore.Mvc;
using Refit;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Lykke.AlgoStore.Api.Infrastructure.Extensions
{
    public static class ErrorExtensions
    {
        public static ObjectResult ToHttpStatusCode(this AlgoStoreException error)
        {
            var errorResponse = new BaseErrorResponse();

            if (error is AlgoStoreAggregateException aggregate)
            {
                errorResponse = new ErrorResponse();
                ((ErrorResponse) errorResponse).ModelErrors = aggregate.Errors;
            }

            errorResponse.ErrorCode = (int) error.ErrorCode;
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
                case AlgoStoreErrorCodes.AlgoInstancesCountLimit:
                case AlgoStoreErrorCodes.BadRequest:
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

            var result = new ObjectResult(errorResponse)
            {
                StatusCode = (int)statusCode
            };

            return result;
        }

        public static AlgoStoreErrorCodes ToAlgoStoreErrorCode(this HttpStatusCode httpStatusCode)
        {
            AlgoStoreErrorCodes errorCode;

            switch (httpStatusCode)
            {
                case HttpStatusCode.BadRequest:
                    errorCode = AlgoStoreErrorCodes.BadRequest;
                    break;

                default:
                    errorCode = AlgoStoreErrorCodes.Unhandled;
                    break;
            }

            return errorCode;
        }

        public static AlgoStoreException ToAlgoStoreException(this ApiException apiException)
        {
            //extract content
            var errorResponse =
                JsonConvert.DeserializeObject<Common.Api.Contract.Responses.ErrorResponse>(apiException.Content);

            var exception = errorResponse != null && !string.IsNullOrEmpty(errorResponse.ErrorMessage)
                ? new AlgoStoreException(apiException.StatusCode.ToAlgoStoreErrorCode(), errorResponse.ErrorMessage,
                    errorResponse.ErrorMessage)
                : new AlgoStoreException(apiException.StatusCode.ToAlgoStoreErrorCode(), apiException.Content,
                    apiException.Content);

            return exception;
        }
    }
}
