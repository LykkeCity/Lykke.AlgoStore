using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IStorageConnectionManager
    {
        Task Refresh();
        StorageConnectionData GetData(string key);
    }
}
