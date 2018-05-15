using System.Threading.Tasks;

namespace Lykke.AlgoStore.Core.Services
{
    public interface ICSharpAlgoBuildService
    {
        Task ExtractMetadata(string content);
    }
}
