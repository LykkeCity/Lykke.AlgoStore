using System.Net;
using Lykke.AlgoStore.Core.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AlgoStore.Api.Infrastructure.Extensions
{
    public static class ErrorExtensions
    {
        public static ObjectResult ToHttpStatusCode(this AlgoStoreError error)
        {
            switch (error.ErrorCode)
            {
                default:
                    //return new BadRequestObjectResult((int)HttpStatusCode.InternalServerError,  new object());
                    return new BadRequestObjectResult( new object()); //new ErrorResponse where we set http code and message?

            }
        }
    }
}
