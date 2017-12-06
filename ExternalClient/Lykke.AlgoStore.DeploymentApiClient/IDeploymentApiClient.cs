using System.Threading.Tasks;

namespace Lykke.AlgoStore.DeploymentApiClient
{
    public interface IDeploymentApiClient : IDeploymentApiReadOnlyClient
    {
        Task<string> BuildAlgoImageFromBinary(byte[] data, string algoUsername, string algoName);
    }
}
