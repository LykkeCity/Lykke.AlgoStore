using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IStorageConnectionManager
    {
        Task Refresh();
    }
}
