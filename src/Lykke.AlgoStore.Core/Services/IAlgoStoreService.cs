using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreService
    {
        Task<bool> DeployImageAsync(ManageImageData data);
        Task<string> StartTestImageAsync(ManageImageData data);
        Task<string> StopTestImageAsync(ManageImageData data);
        Task<string> GetTestTailLogAsync(TailLogData data);
        Task DeleteImageAsync(AlgoClientRuntimeData runtimeData);
    }
}
