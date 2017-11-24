using System.Net;
using Lykke.AlgoStore.Core.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AlgoStore.Api.Infrastructure.Extensions
{
    public static class ErrorExtensions
    {
        public static StatusCodeResult ToHttpStatusCode(this AlgoStoreError error)
        {
            switch (error.ErrorCode)
            {
                default:
                    return new StatusCodeResult((int)HttpStatusCode.InternalServerError);

            }
        }
    }
}
