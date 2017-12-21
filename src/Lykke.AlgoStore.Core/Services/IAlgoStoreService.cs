using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IAlgoStoreService
    {
        Task<bool> DeployImage(ManageImageData data);
        Task<string> StartTestImage(ManageImageData data);
        Task<string> StopTestImage(ManageImageData data);
    }
}
