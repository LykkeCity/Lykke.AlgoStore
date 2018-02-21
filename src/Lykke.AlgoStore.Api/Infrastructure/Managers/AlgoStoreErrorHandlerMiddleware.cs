using System;
using System.IO;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Api.Infrastructure.Managers
{
    public class AlgoStoreErrorHandlerMiddleware
    {
        private readonly ILog _log;
        private readonly string _componentName;
        private readonly RequestDelegate _next;

        public AlgoStoreErrorHandlerMiddleware(RequestDelegate next, ILog log, string componentName)
        {
            _next = next;
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _componentName = componentName ?? throw new ArgumentNullException(nameof(componentName));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await LogError(context, ex);
                await CreateErrorResponse(context, ex);
            }
        }

        private async Task LogError(HttpContext context, Exception ex)
        {
            // request body might be already read at the moment 
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            using (var ms = new MemoryStream())
            {
                context.Request.Body.CopyTo(ms);

                ms.Seek(0, SeekOrigin.Begin);

                await _log.LogPartFromStream(ms, _componentName, context.Request.GetUri().AbsoluteUri, ex);
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
