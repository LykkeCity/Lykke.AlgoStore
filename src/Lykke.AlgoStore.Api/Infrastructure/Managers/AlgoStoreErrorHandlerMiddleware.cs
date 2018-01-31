using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Api.Infrastructure.Managers
{
    public class AlgoStoreErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public AlgoStoreErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await CreateErrorResponse(context, ex);
            }
        }

        private async Task CreateErrorResponse(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";

            var response = ExceptionManager.CreateErrorResult(ex);
            if (response == null || !response.StatusCode.HasValue)
                throw ex;

            ctx.Response.StatusCode = response.StatusCode.Value;

            var responseJson = JsonConvert.SerializeObject(response.Value);

            await ctx.Response.WriteAsync(responseJson);
        }
    }
}
