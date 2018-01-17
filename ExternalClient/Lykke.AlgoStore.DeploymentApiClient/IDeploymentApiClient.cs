using System.Threading.Tasks;

namespace Lykke.AlgoStore.DeploymentApiClient
{
    public interface IDeploymentApiClient : IDeploymentApiReadOnlyClient
    {
        Task<string> BuildAlgoImageFromBinaryAsync(byte[] data, string algoUsername, string algoName);
        Task<long> CreateTestAlgoAsync(long imageId, string algoId);
        Task<bool> StartTestAlgoAsync(long imageId);
        Task<bool> StopTestAlgoAsync(long imageId);
        Task<string> GetTestAlgoLogAsync(long imageId);
        Task<string> GetTestAlgoTailLogAsync(long imageId, int tail);
        Task<bool> DeleteAlgoAsync(long imageId);
        Task<bool> DeleteTestAlgoAsync(long imageId);
    }
}
