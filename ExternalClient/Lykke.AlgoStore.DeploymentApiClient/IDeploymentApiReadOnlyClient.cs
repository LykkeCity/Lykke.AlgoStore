using System.Threading.Tasks;
using Lykke.AlgoStore.DeploymentApiClient.Models;

namespace Lykke.AlgoStore.DeploymentApiClient
{
    public interface IDeploymentApiReadOnlyClient
    {
        Task<ClientAlgoRuntimeStatuses> GetAlgoTestAdministrativeStatus(long id);
        Task<bool> DeleteAlgo(long imageId);
        Task<bool> StopTestAlgo(long imageId);
        Task<bool> DeleteTestAlgo(long imageId);
    }
}
