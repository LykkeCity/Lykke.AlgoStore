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

        Task<Iok8skubernetespkgapiv1Namespace> CreateNamespaceAsync(
            Iok8skubernetespkgapiv1Namespace body, string pretty = default(string),
            CancellationToken cancellationToken = default(CancellationToken));

        Task<Iok8sapimachinerypkgapismetav1Status> DeleteNamespaceAsync(
            Iok8sapimachinerypkgapismetav1DeleteOptions body, string name, int? gracePeriodSeconds = default(int?),
            bool? orphanDependents = default(bool?), string propagationPolicy = default(string),
            string pretty = default(string), CancellationToken cancellationToken = default(CancellationToken));
    }
}
