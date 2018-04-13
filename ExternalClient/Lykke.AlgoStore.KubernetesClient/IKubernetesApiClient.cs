using System.Threading.Tasks;
using Lykke.AlgoStore.KubernetesClient.Models;

namespace Lykke.AlgoStore.KubernetesClient
{
    public interface IKubernetesApiClient : IKubernetesApiReadOnlyClient
    {
        Task<bool> DeleteAsync(string instanceId, string podNamespace);
        Task<string> ReadPodLogAsync(Iok8skubernetespkgapiv1Pod pod, int? tailLines);
    }
}
