using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoClientInstanceRepository : IAlgoClientInstanceReadOnlyRepository
    {
        Task SaveAlgoInstanceDataAsync(AlgoClientInstanceData data);
        Task DeleteAlgoInstanceDataAsync(AlgoClientInstanceData metaData);
    }
}
