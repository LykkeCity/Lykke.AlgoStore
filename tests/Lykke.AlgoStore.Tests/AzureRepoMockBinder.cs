using Autofac;
using AzureStorage.Blob;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.SettingsReader;
using Moq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Tests
{
    public static class AzureRepoMockBinder
    {
        public static void BindAzureReposInMemForTests(this ContainerBuilder ioc)
        {
            //ioc.RegisterInstance<IAlgoBlobRepository<byte[]>>(new AlgoBlobRepository(new AzureBlobInMemory())).Named<IAlgoBlobRepository<byte[]>>("InMemoryRepo");
            //ioc.RegisterInstance<IAlgoBlobRepository<string>>(new AlgoBlobStringRepository(new AzureBlobInMemory()));

            //var reloadingMock = new Mock<IReloadingManager<string>>();
            //reloadingMock
            //    .Setup(x => x.Reload())
            //    .Returns(() => Task.FromResult("DefaultEndpointsProtocol=https;AccountName=lkedevmain;AccountKey=l0W0CaoNiRZQIqJ536sIScSV5fUuQmPYRQYohj/UjO7+ZVdpUiEsRLtQMxD+1szNuAeJ351ndkOsdWFzWBXmdw=="));

            //ioc.RegisterInstance(reloadingMock.Object);

            //ioc.RegisterInstance<IAlgoBlobRepository<byte[]>>(new AlgoBlobRepository(AzureBlobStorage.Create(reloadingMock.Object.ConnectionString(x => x))) ).Named<IAlgoBlobRepository<byte[]>>("RealStorageRepo");
        }
    }
}
