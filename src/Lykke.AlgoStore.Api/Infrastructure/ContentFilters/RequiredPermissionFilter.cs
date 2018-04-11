using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
namespace Lykke.AlgoStore.Api.Infrastructure.ContentFilters
{
    public class RequiredPermissionFilter: ActionFilterAttribute
    {
        public string Permission { get; set; }

        public RequiredPermissionFilter(string permission)
        {
            Permission = permission;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRoles = context.HttpContext.User.GetRoles();
            var hasRole = false;
            for (var i = 0; i < userRoles.Count; i++)
            {
                if (userRoles[i].Permissions.Find(p => p.Name == Permission) != null)
                {
                    hasRole = true;
                    break;
                }
            }

            if(!hasRole)
            {
                context.Result = new StatusCodeResult(403);
            }

        }
    }
}
