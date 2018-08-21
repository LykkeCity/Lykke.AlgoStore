using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Service.Security.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Lykke.AlgoStore.Job.GDPR.Client;

namespace Lykke.AlgoStore.Api.Infrastructure.ContentFilters
{
    public class PermissionFilter: IActionFilter
    {
        private readonly ISecurityClient _securityClient;
        private readonly IGdprClient _gdprClient;

        public PermissionFilter(ISecurityClient securityClient, IGdprClient gdprClient)
        {
            _securityClient = securityClient;
            _gdprClient = gdprClient;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var clientId = context.HttpContext.User.GetClientId();


           // var user = _usersService.GetByIdAsync(clientId).Result;            

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

                var legalConsent = _gdprClient.GetLegalConsentsAsync(clientId).Result;

                if(legalConsent == null || !legalConsent.CookieConsent || !legalConsent.GdprConsent)
                    context.Result = new StatusCodeResult(451);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
