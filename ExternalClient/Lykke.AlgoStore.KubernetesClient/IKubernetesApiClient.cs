using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.KubernetesClient.Models;

namespace Lykke.AlgoStore.KubernetesClient
{
    public interface IKubernetesApiClient
    {
        Task<IList<Iok8skubernetespkgapiv1Pod>> ListPodsByAlgoIdAsync(string algoId);
        Task<Iok8sapimachinerypkgapismetav1Status> DeleteDeploymentAsync(string algoId, Iok8skubernetespkgapiv1Pod pod);
        Task<string> ReadPodLogAsync(Iok8skubernetespkgapiv1Pod pod, int? tailLines);
    }
}
