using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Lykke.AlgoStore.Api.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static T GetHeaderValueAs<T>(this HttpContext httpContext, string headerName)
        {
            StringValues values;

            if (httpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!string.IsNullOrEmpty(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }
    }
}
