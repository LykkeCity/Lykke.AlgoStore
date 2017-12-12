using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.DeploymentApiClient.Models;

namespace Lykke.AlgoStore.Services.Utils
{
    public static class RuntimeStatusMapper
    {
        public static AlgoRuntimeStatuses ToModel(this ClientAlgoRuntimeStatuses status)
        {
            switch (status)
            {
                case ClientAlgoRuntimeStatuses.Created:
                    return AlgoRuntimeStatuses.Deployed;
                case ClientAlgoRuntimeStatuses.Paused:
                    return AlgoRuntimeStatuses.Paused;
                case ClientAlgoRuntimeStatuses.Running:
                    return AlgoRuntimeStatuses.Started;
                case ClientAlgoRuntimeStatuses.Stopped:
                    return AlgoRuntimeStatuses.Stopped;
                default:
                    return AlgoRuntimeStatuses.Unknown;
            }
        }
    }
}
