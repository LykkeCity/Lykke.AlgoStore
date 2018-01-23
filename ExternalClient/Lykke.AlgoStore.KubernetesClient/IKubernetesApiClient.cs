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

        Task<DeleteNamespaceResponse> DeleteNamespaceAsync(
            Iok8sapimachinerypkgapismetav1DeleteOptions body, string name, int? gracePeriodSeconds = default(int?),
            bool? orphanDependents = default(bool?), string propagationPolicy = default(string),
            string pretty = default(string), CancellationToken cancellationToken = default(CancellationToken));

        Task<string> ReadPodLogAsync(string name, string namespaceParameter,
            string container = default(string), bool? follow = default(bool?), int? limitBytes = default(int?),
            string pretty = default(string), bool? previous = default(bool?), int? sinceSeconds = default(int?),
            int? tailLines = default(int?), bool? timestamps = default(bool?),
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
