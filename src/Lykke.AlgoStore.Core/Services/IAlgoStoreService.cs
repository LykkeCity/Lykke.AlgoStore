using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;
using AlgoClientInstanceData = Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoClientInstanceData;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreService
    {
        Task<bool> DeployImageAsync(ManageImageData data);
        Task<string> StopTestImageAsync(ManageImageData data);
        Task<string[]> GetTestTailLogAsync(TailLogData data);
        Task DeleteInstanceAsync(AlgoClientInstanceData instanceData);
        Task<bool> UpdateAlgoInstanceStatusAsync(TeamCityWebHookResponse payload);
    }
}
