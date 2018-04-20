using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.Infrastructure.ContentFilters
{
    public class PermissionFilter: IActionFilter
    {
        private readonly IUserRolesService _roleService;

        public PermissionFilter(IUserRolesService roleService)
        {
            _roleService = roleService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var clientId = context.HttpContext.User.GetClientId();
            var userRoles = _roleService.GetRolesByClientIdAsync(clientId).Result;
            var requiredPermission = (context.ActionDescriptor as ControllerActionDescriptor)?.ActionName;

            // Check if the action needs a permission
            var reflectedMethod = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            var needsPermission = reflectedMethod?.GetCustomAttributes(typeof(RequirePermissionAttribute), false).FirstOrDefault() != null || reflectedMethod?.ReflectedType.GetCustomAttributes(typeof(RequirePermissionAttribute), false).FirstOrDefault() != null;

            if (needsPermission)
            {
                var hasPermission = userRoles.Any(x => x.Permissions.Any(y => y.Id == requiredPermission));

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
