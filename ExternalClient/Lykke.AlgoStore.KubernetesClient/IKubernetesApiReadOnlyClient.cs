using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.KubernetesClient.Models;

namespace Lykke.AlgoStore.KubernetesClient
{
    public interface IKubernetesApiReadOnlyClient
    {
        Task<IList<Iok8skubernetespkgapiv1Pod>> ListPodsByAlgoIdAsync(string instanceId);
    }
}
