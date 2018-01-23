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
        [Test]
        public async Task GetNamespaces_Returns_Success()
        {
            var client = Given_KubernetesClient();
            var list = await client.ListCoreV1NamespaceAsync();

            Then_Result_Should_Contain_NamespacesData(list);
        }

        [Test, Explicit("Run manually cause it will create new deployment with specific name")]
        public async Task DeployPod_Returns_DeploymentData()
        {
            var client = Given_KubernetesClient();

            var result = await client.CreateDeploymentAsync(
                new Iok8skubernetespkgapisappsv1beta1Deployment
                {
                    ApiVersion = "apps/v1beta1",
                    Kind = "Deployment",
                    Metadata = new Iok8sapimachinerypkgapismetav1ObjectMeta
                    {
                        Name = "deployment-example" //Unique key of the Deployment instance
                    },
                    Spec = new Iok8skubernetespkgapisappsv1beta1DeploymentSpec
                    {
                        Replicas = 1,
                        RevisionHistoryLimit = 10,
                        Template = new Iok8skubernetespkgapiv1PodTemplateSpec
                        {
                            Metadata = new Iok8sapimachinerypkgapismetav1ObjectMeta
                            {
                                Labels = new Dictionary<string, string>()
                                {
                                    {"app", "nginx"}
                                }
                            },
                            Spec = new Iok8skubernetespkgapiv1PodSpec
                            {
                                Containers = new List<Iok8skubernetespkgapiv1Container>
                                {
                                    new Iok8skubernetespkgapiv1Container
                                    {
                                        Name = "nginx",
                                        Image = "crccheck/hello-world", //hello-world",//"nginx:1.10",
                                        Ports = new List<Iok8skubernetespkgapiv1ContainerPort>
                                        {
                                            new Iok8skubernetespkgapiv1ContainerPort
                                            {
                                                ContainerPort = 80
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                "default"
            );

            Then_Result_Should_Contain_DeploymentData(result);
        }

        [Test,
         Explicit("Run manually cause it will try to delete existing pod (one that is created via DeployNewPod test")]
        public async Task DeletePod_Returns_StatusData()
        {
            var client = Given_KubernetesClient();

            var result = await client.DeleteAppsV1beta1NamespacedDeploymentAsync(
                new Iok8sapimachinerypkgapismetav1DeleteOptions
                {
                    GracePeriodSeconds = 0,
                    OrphanDependents = false
                },
                "deployment-example",
                "default" //,
                //REMARK: I spent too much time until I figured out that parameters from below should be set inside body!!!
                //0,
                //false
            );

            Then_Result_Should_Contain_StatusData(result);
        }

        [Test, Explicit("Run manually cause it will create new namespace with specific name")]
        public async Task CreateNamespace_Returns_NamespaceData()
        {
            var client = Given_KubernetesClient();

            var result = await client.CreateNamespaceAsync(
                new Iok8skubernetespkgapiv1Namespace
                {
                    Metadata = new Iok8sapimachinerypkgapismetav1ObjectMeta
                    {
                        Name = "mj-test"
                    }
                }
            );

            Then_Result_Should_Contain_NamespaceData(result);
        }

        [Test,
         Explicit(
             "Run manually cause it will try to delete existing namespace (one that is created via CreateNamespace test")]
        public async Task DeleteNamespace_Returns_DeleteNamespaceData()
        {
            var client = Given_KubernetesClient();

            var result = await client.DeleteNamespaceAsync(
                new Iok8sapimachinerypkgapismetav1DeleteOptions
                {
                    GracePeriodSeconds = 0,
                    OrphanDependents = false
                },
                "mj-test"
            );

            Then_Result_Should_Contain_DeleteNamespaceData(result);
        }

        [Test, Explicit("Run manually cause it will try to get log for existing pod")]
        public async Task GetPodLog_Returns_LogData()
        {
            var client = Given_KubernetesClient();

            var result = await client.ReadPodLogAsync(
                "kubernetes-dashboard-924040265-3hgnr", //pod name
                "kube-system", // pod namespace
                tailLines: 100 // get last 100 lines from log
            );

            Then_Result_Should_Contain_LogData(result);
        }

        [Test, Explicit("Run manually cause it will try to get existing pod")]
        public async Task GetPod_Returns_PodData()
        {
            var client = Given_KubernetesClient();

            var result = await client.ReadCoreV1NamespacedPodAsync(
                "kubernetes-dashboard-924040265-3hgnr", //pod name
                "kube-system" // pod namespace
            );

            Then_Result_Should_Contain_PodData(result);
        }

        private static void Then_Result_Should_Contain_PodData(Iok8skubernetespkgapiv1Pod result)
        {
            Assert.IsNotNull(result);
        }

        private static void Then_Result_Should_Contain_LogData(string result)
        {
            Assert.IsNotNull(result);
        }

        private static void Then_Result_Should_Contain_NamespacesData(Iok8skubernetespkgapiv1NamespaceList list)
        {
            Assert.IsNotNull(list);
            Assert.IsNotNull(list.Items);
            Assert.Greater(list.Items.Count, 0);
        }

        private static KubernetesApiClient Given_KubernetesClient()
        {
            var client = new KubernetesApiClient
            {
                BaseUri = new System.Uri("http://127.0.0.1:8001")
            };

            client.SetRetryPolicy(null);

            return client;
        }

        private static void Then_Result_Should_Contain_DeploymentData(Iok8skubernetespkgapisappsv1beta1Deployment result)
        {
            Assert.IsNotNull(result);
        }

        private static void Then_Result_Should_Contain_StatusData(Iok8sapimachinerypkgapismetav1Status result)
        {
            Assert.IsNotNull(result);
        }

        private static void Then_Result_Should_Contain_NamespaceData(Iok8skubernetespkgapiv1Namespace result)
        {
            Assert.IsNotNull(result);
        }

        private static void Then_Result_Should_Contain_DeleteNamespaceData(DeleteNamespaceResponse result)
        {
            Assert.IsNotNull(result);
        }
    }
}
