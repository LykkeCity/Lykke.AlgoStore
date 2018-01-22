using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.KubernetesClient.Models;

namespace Lykke.AlgoStore.KubernetesClient
{
    public interface IKubernetesApiClient
    {
        Task<Iok8skubernetespkgapisappsv1beta1Deployment> CreateDeploymentAsync(
            Iok8skubernetespkgapisappsv1beta1Deployment body, string namespaceParameter,
            string pretty = default(string), CancellationToken cancellationToken = default(CancellationToken));
    }
}
