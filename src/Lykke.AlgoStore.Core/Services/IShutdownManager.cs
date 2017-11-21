using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
