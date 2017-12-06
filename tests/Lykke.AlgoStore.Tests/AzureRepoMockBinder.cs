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
            ioc.RegisterInstance<IAlgoBlobRepository<byte[]>>(new AlgoBlobBinaryRepository(new AzureBlobInMemory())).Named<IAlgoBlobRepository<byte[]>>("InMemoryRepo");
            ioc.RegisterInstance<IAlgoBlobRepository<string>>(new AlgoBlobStringRepository(new AzureBlobInMemory()));

            var reloadingMock = new Mock<IReloadingManager<string>>();
            reloadingMock
                .Setup(x => x.Reload())
                .Returns(() => Task.FromResult("DefaultEndpointsProtocol=https;AccountName=algostoredev;AccountKey=d2VaBHrf8h8t622KvLeTPGwRP4Dxz9DTPeBT9H3zmjcQprQ1e+Z6Sx9RDqJc+zKwlSO900fzYF2Dg6oUBVDe1w=="));

            ioc.RegisterInstance(reloadingMock.Object);

            ioc.RegisterInstance<IAlgoBlobRepository<byte[]>>(new AlgoBlobBinaryRepository(AzureBlobStorage.Create(reloadingMock.Object.ConnectionString(x => x))) ).Named<IAlgoBlobRepository<byte[]>>("RealStorageRepo");
        }
    }
}
