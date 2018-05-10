using System;

namespace Lykke.AlgoStore.Api.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequirePermissionAttribute: Attribute
    {

    }
}
