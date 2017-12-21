using System.Threading.Tasks;
using Lykke.AlgoStore.DeploymentApiClient.Models;

namespace Lykke.AlgoStore.DeploymentApiClient
{
    public interface IDeploymentApiReadOnlyClient
    {
        Task<ClientAlgoRuntimeStatuses> GetAlgoTestAdministrativeStatus(long id);
    }
}
