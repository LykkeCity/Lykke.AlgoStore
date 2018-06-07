using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Lykke.AlgoStore.Service.Security.Client;

namespace Lykke.AlgoStore.Api.Infrastructure.ContentFilters
{
    public class PermissionFilter: IActionFilter
    {
        private readonly ISecurityClient _securityClient;

        public PermissionFilter(ISecurityClient securityClient)
        {
            _securityClient = securityClient;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var clientId = context.HttpContext.User.GetClientId();
            var requiredPermission = (context.ActionDescriptor as ControllerActionDescriptor)?.ActionName;
            var reflectedMethod = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            var needsPermission =
                reflectedMethod?.GetCustomAttributes(typeof(RequirePermissionAttribute), false).FirstOrDefault() != null 
                || reflectedMethod?.ReflectedType.GetCustomAttributes(typeof(RequirePermissionAttribute), false).FirstOrDefault() != null;

            if (needsPermission)
            {
                var hasPermission = _securityClient.HasPermissionAsync(clientId, requiredPermission).Result;

                if (!hasPermission)
                {
                    context.Result = new StatusCodeResult(403);
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
