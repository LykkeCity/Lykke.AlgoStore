using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoMetaDataRepository : IAlgoMetaDataReadOnlyRepository
    {
        Task SaveAlgoMetaData(AlgoClientMetaData metaData);
        Task DeleteAlgoMetaData(AlgoClientMetaData metaData);
    }
}
