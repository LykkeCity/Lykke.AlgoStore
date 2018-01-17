using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoMetaDataRepository : IAlgoMetaDataReadOnlyRepository
    {
        Task SaveAlgoMetaDataAsync(AlgoClientMetaData metaData);
        Task DeleteAlgoMetaDataAsync(AlgoClientMetaData metaData);
    }
}
