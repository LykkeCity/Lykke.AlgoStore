using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.KubernetesClient.Models;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class KubernetsClientTests
    {
        private const string Id = "test3";
        //private const string Id = "b0ea081e-470d-462c-99a7-7864f13a7ddb";

        [Test, Explicit("Run manually cause it will try to get pods")]
        public void ListPodByAlgoId_Returns_Success()
        {
            IKubernetesApiClient client = Given_KubernetesClient();
            var result = When_I_Call_ListPodsByAlgoIdAsync(client).Result;
            Then_Result_ShouldBe_Valid(result);
        }

        [Test, Explicit("Run manually cause it will try to delete deployment")]
        public void DeleteDeployment_Returns_Success()
        {
            var client = Given_KubernetesClient();
            var result = When_I_Call_ListPodsByAlgoIdAsync(client).Result;
            Then_Result_ShouldBe_Valid(result);
            var pod = result[0];

            var status = When_I_Call_DeleteDeploymentAsync(client, pod).Result;
            Then_Result_ShouldBe_True(status);
        }

        [Test, Explicit("Run manually cause it will try to delete service")]
        public void DeleteService_Returns_Success()
        {
            var client = Given_KubernetesClient();
            var result = When_I_Call_ListPodsByAlgoIdAsync(client).Result;
            Then_Result_ShouldBe_Valid(result);
            var pod = result[0];

            var status = When_I_Call_DeleteServiceAsync(client, pod).Result;
            Then_Result_ShouldBe_True(status);
        }

        [Test, Explicit("Run manually cause it will try to get log for existing pod")]
        public void GetPodLog_Returns_LogData()
        {
            var client = Given_KubernetesClient();
            var result = When_I_Call_ListPodsByAlgoIdAsync(client).Result;
            Then_Result_ShouldBe_Valid(result);
            var pod = result[0];

            var log = When_I_Call_ReadPodLogAsync(client, pod).Result;
            Then_Result_Should_Contain_LogData(log);
        }

        #region Private Methods
        private static KubernetesApiClient Given_KubernetesClient()
        {
            var client = new KubernetesApiClient
            {
                BaseUri = new System.Uri("http://127.0.0.1:8001")
            };

            client.SetRetryPolicy(null);

            return client;
        }
        private async Task<IList<Iok8skubernetespkgapiv1Pod>> When_I_Call_ListPodsByAlgoIdAsync(IKubernetesApiClient client)
        {
            return await client.ListPodsByAlgoIdAsync(Id);
        }
        private async Task<string> When_I_Call_ReadPodLogAsync(IKubernetesApiClient client, Iok8skubernetespkgapiv1Pod pod)
        {
            return await client.ReadPodLogAsync(pod, 10);
        }
        private async Task<bool> When_I_Call_DeleteDeploymentAsync(KubernetesApiClient client, Iok8skubernetespkgapiv1Pod pod)
        {
            return await client.DeleteDeploymentAsync(Id, pod);
        }
        private async Task<bool> When_I_Call_DeleteServiceAsync(KubernetesApiClient client, Iok8skubernetespkgapiv1Pod pod)
        {
            return await client.DeleteServiceAsync(Id, pod);
        }
        private static void Then_Result_ShouldBe_Valid(IList<Iok8skubernetespkgapiv1Pod> pods)
        {
            Assert.IsNotNull(pods);
            Assert.AreEqual(1, pods.Count);
            Assert.IsNotNull(pods[0]);
        }
        private static void Then_Result_Should_Contain_LogData(string result)
        {
            Assert.IsNotNull(result);
        }
        private static void Then_Result_ShouldBe_True(bool status)
        {
            Assert.IsTrue(status);
        }
        #endregion
    }
}
