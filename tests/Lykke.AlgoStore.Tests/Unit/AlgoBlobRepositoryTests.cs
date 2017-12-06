using Autofac;
using Common.Log;
using Lykke.AlgoStore.Core.Domain.Repositories;
using System;
using System.Linq;
using System.Text;
using Lykke.AlgoStore.Tests.Infrastructure;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    public class AlgoBlobRepositoryTests
    {
        private static IContainer _ioc;
        private static string blobKey = "TestKey";
        private static byte[] blobBytes = Encoding.Unicode.GetBytes(blobKey);

        [SetUp]
        public void SetUp()
        {
            var ioc = new ContainerBuilder();
            ioc.Register(x => new LogToMemory()).As<ILog>();
            ioc.BindAzureReposInMemForTests();
            _ioc = ioc.Build();
        }

        [RunnableInDebugOnly("Should run manually only. Manipulate data in Table Storage")]
        [Test]
        public void Blob_Large_Binary_Save_Test()
        {
            var repo = Given_Algo_RealBlob_Starage_Repository();
            var largeByteArray = Give_Large_Byte_Array();
            When_Invoke_Save_BinaryFile(repo, blobKey, largeByteArray);
            Then_BinaryFile_ShouldBe(repo, blobKey, largeByteArray);
            Then_DeleteBinary(repo, blobKey);
        }

        [Test]
        public void BlobBinary_Save_Test()
        {
            var repo = Given_AlgoBinary_InMemory_Storage_Repository();
            When_Invoke_Save_BinaryFile(repo, blobKey, blobBytes);
            Then_BinaryFile_ShouldBe(repo, blobKey, blobBytes);
            And_BinaryFileToString_ShouldBe(repo, blobKey);
        }

        [Test]
        public void BlobBinary_Exists_Test()
        {
            var repo = Given_AlgoBinary_InMemory_Storage_Repository();
            When_Invoke_Save_BinaryFile(repo, blobKey, blobBytes);
            Then_BinaryFile_ShouldBe(repo, blobKey, blobBytes);
        }

        [Test]
        public void BlobBinary_Delete_Test()
        {
            var repo = Given_AlgoBinary_InMemory_Storage_Repository();
            When_Invoke_Save_BinaryFile(repo, blobKey, blobBytes);
            And_TryDelete_BinaryFile(repo, blobKey);
            Then_BinaryFile_ShouldNotExist(repo, blobKey);
        }

        #region Private Methods

        private static void Then_DeleteBinary(IAlgoBlobRepository<byte[]> repo, string blobKey)
        {
            repo.DeleteBlobAsync(blobKey).Wait();
            Assert.False(repo.BlobExists(blobKey).Result);
        }

        private static byte[] Give_Large_Byte_Array()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[100000000];
            rnd.NextBytes(b);
            return b;
        }

        private static void Then_BinaryFile_ShouldNotExist(IAlgoBlobRepository<byte[]> repo, string blobKey)
        {
            var exists = repo.BlobExists(blobKey).Result;
            Assert.False(exists);
        }

        private static IAlgoBlobRepository<byte[]> Given_AlgoBinary_InMemory_Storage_Repository()
        {
            return _ioc.ResolveNamed<IAlgoBlobRepository<byte[]>>("InMemoryRepo");
        }

        private static IAlgoBlobRepository<byte[]> Given_Algo_RealBlob_Starage_Repository()
        {
            return _ioc.ResolveNamed<IAlgoBlobRepository<byte[]>>("RealStorageRepo");
        }

        private static IAlgoBlobRepository<string> Given_AlgoString_Repository()
        {
            return _ioc.Resolve<IAlgoBlobRepository<string>>();
        }

        private static void When_Invoke_Save_BinaryFile(IAlgoBlobRepository<byte[]> repository, string key, byte[] bytes)
        {
            repository.SaveBlobAsync(key, bytes).Wait();
        }

        private static void And_TryDelete_BinaryFile(IAlgoBlobRepository<byte[]> repository, string key)
        {
            repository.DeleteBlobAsync(key).Wait();
        }

        private static void Then_BinaryFile_ShouldBe(IAlgoBlobRepository<byte[]> repository, string key, byte[] bytes)
        {
            var saved = repository.GetBlobAsync(key).Result;
            Assert.True(saved.SequenceEqual(bytes));
        }

        private static void Then_BinaryFile_ShouldExist(IAlgoBlobRepository<byte[]> repository, string key)
        {
            var saved = repository.BlobExists(key).Result;
            Assert.True(saved);
        }

        private static void And_BinaryFileToString_ShouldBe(IAlgoBlobRepository<byte[]> repository, string key)
        {
            var saved = repository.GetBlobAsync(key).Result;
            Assert.True(Encoding.Unicode.GetString(saved) == key);
        }

        #endregion
    }
}
