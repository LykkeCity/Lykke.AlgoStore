using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage.Tables;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.KubernetesClient;
using Lykke.AlgoStore.KubernetesClient.Models;
using Lykke.AlgoStore.Tests.Infrastructure;
using Microsoft.Rest;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class KubernetsClientTests
    {
        //private const string Id = "test3";
        private const string Id = "aab7cff6-8690-47d3-a0db-8e84bdad4b03";

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

        [Test, Explicit("Run manually cause it will try to delete service")]
        public void DeleteServiceWithoutPod_Returns_Success()
        {
            var client = Given_KubernetesClient();
            var namespaceParameter = "algo-test";
            
            var status = When_I_Call_DeleteServiceAsync(client, namespaceParameter).Result;
            Then_Result_ShouldBe_True(status);
        }

        [Test, Explicit("Run manually cause it will try to delete multiple services")]
        public void DeleteMultipleServicesWithoutPods_Returns_Success()
        {
            var client = Given_KubernetesClient();
            var namespaceParameter = "algo-test";
            var servicesToDelete = new List<string>
            {
                "0016daf7-63cb-4501-8e7f-077f3f3d51af",
                "00386586-17a4-4584-8430-5d03ae1d656d"
            };

            foreach (var serviceToDetele in servicesToDelete)
            {
                var status = When_I_Call_DeleteServiceAsync(client, serviceToDetele, namespaceParameter).Result;
                Then_Result_ShouldBe_True(status);
            }
        }

        #region Private Methods
        private static UserLogRepository Given_UserLog_Repository()
        {
            return new UserLogRepository(
                AzureTableStorage<UserLogEntity>.Create(SettingsMock.GetLogsConnectionString(), UserLogRepository.TableName, new LogMock())
            );
        }

        private static KubernetesApiClient Given_KubernetesClient()
        {
            var repo = Given_UserLog_Repository();
            var trt = SettingsMock.GetKubeBasicAuthenticationValue().CurrentValue;

            var client = new KubernetesApiClient(new System.Uri("http://127.0.0.1:8001"),
                new TokenCredentials(SettingsMock.GetKubeBasicAuthenticationValue().CurrentValue), "", repo);

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
            return await client.DeleteDeploymentAsync(Id, pod.Metadata.NamespaceProperty);
        }
        private async Task<bool> When_I_Call_DeleteServiceAsync(KubernetesApiClient client, Iok8skubernetespkgapiv1Pod pod)
        {
            return await client.DeleteServiceAsync(Id, pod.Metadata.NamespaceProperty);
        }

        private async Task<bool> When_I_Call_DeleteServiceAsync(KubernetesApiClient client, string namespaceParameter)
        {
            return await client.DeleteServiceAsync(Id, namespaceParameter);
        }

        private async Task<bool> When_I_Call_DeleteServiceAsync(KubernetesApiClient client, string name, string namespaceParameter)
        {
            return await client.DeleteServiceAsync(name, namespaceParameter);
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
