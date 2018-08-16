﻿using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.AlgoStore.Core.Services;

namespace Lykke.AlgoStore.Api.Infrastructure.ContentFilters
{
    public class PermissionFilter: IActionFilter
    {
        private readonly ISecurityClient _securityClient;
        private readonly IUsersService _usersService;
 
        public PermissionFilter(ISecurityClient securityClient, IUsersService usersService)
        {
            _securityClient = securityClient;
            _usersService = usersService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var clientId = context.HttpContext.User.GetClientId();

            var user = _usersService.GetByIdAsync(clientId).Result;            

            var requiredPermission = (context.ActionDescriptor as ControllerActionDescriptor)?.ActionName;
            var reflectedMethod = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            var needsPermission =
                reflectedMethod?.GetCustomAttributes(typeof(RequirePermissionAttribute), false).FirstOrDefault() != null 
                || reflectedMethod?.ReflectedType.GetCustomAttributes(typeof(RequirePermissionAttribute), false).FirstOrDefault() != null;

            if (needsPermission)
            {
                var hasPermission = _securityClient.HasPermissionAsync(clientId, requiredPermission).Result;

                if (user == null || !user.GDPRConsent)
                {
                    context.Result = new StatusCodeResult(451);
                }

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
