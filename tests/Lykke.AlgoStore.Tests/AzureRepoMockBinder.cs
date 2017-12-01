using Autofac;
using AzureStorage.Blob;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Tests
{
    public static class AzureRepoMockBinder
    {
        public static void BindAzureReposInMemForTests(this ContainerBuilder ioc)
        {
            ioc.RegisterInstance<IAlgoBlobRepository<byte[]>>(new AlgoBlobBinaryRepository(new AzureBlobInMemory()));
            ioc.RegisterInstance<IAlgoBlobRepository<string>>(new AlgoBlobStringRepository(new AzureBlobInMemory()));
        }
    }
}
