namespace Lykke.AlgoStore.DeploymentApiClient.Models
{
    public enum AlgoRuntimeStatuses
    {
        Success = 0,
        InternalError,
        NotFound,
        Forbidden,
        Unauthorized,
        Running,
        Stopped,
        Paused,
        Created
    }
}
