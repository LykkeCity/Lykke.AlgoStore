using System.Threading.Tasks;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.KubernetesClient.Models;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class KubernetsClientTests
    {
        [Test]
        public async Task GetNamespaces_ReturnSuccess()
        {
            var client = Given_KubernetesClient();
            var list = await client.ListCoreV1NamespaceWithHttpMessagesAsync();

            Then_Result_ShouldContainNamespaceData(list);
        }

        private static void Then_Result_ShouldContainNamespaceData(Microsoft.Rest.HttpOperationResponse<Iok8skubernetespkgapiv1NamespaceList> list)
        {
            Assert.IsNotNull(list);
            Assert.IsNotNull(list.Body.Items);
            Assert.Greater(list.Body.Items.Count, 0);
        }

        private static IKubernetes Given_KubernetesClient()
        {
            var client = new Kubernetes();
            client.SetRetryPolicy(null);

            return client;
        }
    }
}
