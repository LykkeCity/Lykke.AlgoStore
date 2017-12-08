using System;
using System.Linq;
using Lykke.AlgoStore.Core.Utils;
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
        public static string GetIp(this HttpContext context)
        {
            var ip = string.Empty;

            // http://stackoverflow.com/a/43554000/538763
            var xForwardedForVal = context.GetHeaderValueAs<string>("X-Forwarded-For").SplitTrimmed(StringExtensions.CommaSeparator).FirstOrDefault();

            if (!string.IsNullOrEmpty(xForwardedForVal))
            {
                ip = xForwardedForVal.Split(':')[0];
            }

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (string.IsNullOrWhiteSpace(ip) && context.Connection?.RemoteIpAddress != null)
                ip = context.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrWhiteSpace(ip))
                ip = context.GetHeaderValueAs<string>("REMOTE_ADDR");

            return ip;
        }
        public static string GetUserAgent(this HttpContext context)
        {
            return context.Request.GetUserAgent();
        }
    }
}
