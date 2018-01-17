using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoClientInstanceRepository : IAlgoClientInstanceReadOnlyRepository
    {
        Task SaveAlgoInstanceData(AlgoClientInstanceData data);
        Task DeleteAlgoInstanceData(AlgoClientInstanceData metaData);
    }
}
