using System.Threading.Tasks;

namespace Lykke.AlgoStore.DeploymentApiClient
{
    public interface IDeploymentApiClient : IDeploymentApiReadOnlyClient
    {
        Task<string> BuildAlgoImageFromBinary(byte[] data, string algoUsername, string algoName);

        Task<long> CreateTestAlgo(long imageId, string algoId);
        Task<bool> StartTestAlgo(long imageId);
        Task<string> GetTestAlgoLog(long imageId);
    }
}
