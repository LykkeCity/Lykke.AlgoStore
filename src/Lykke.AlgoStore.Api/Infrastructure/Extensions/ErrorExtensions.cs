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

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            switch (error.ErrorCode)
            {
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
